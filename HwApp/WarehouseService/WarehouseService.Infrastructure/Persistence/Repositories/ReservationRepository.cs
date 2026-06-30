using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using WarehouseService.Application.Reservations;
using WarehouseService.Application.Reservations.Operations;
using WarehouseService.Domain.StockReservations;
using WarehouseService.Domain.Stocks;
using WarehouseService.Infrastructure.Messaging;
using WarehouseService.Infrastructure.Messaging.Kafka;
using WarehouseService.Infrastructure.Persistence.Outbox;

namespace WarehouseService.Infrastructure.Persistence.Repositories
{
    internal sealed class ReservationRepository : IReservationRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly KafkaOptions _kafkaOptions;

        public ReservationRepository(
            DatabaseContext databaseContext,
            IOptions<KafkaOptions> kafkaOptions)
        {
            _databaseContext = databaseContext;
            _kafkaOptions = kafkaOptions.Value;
        }

        public async Task<CancelReservationOperationResult> CancelAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var reservation = await _databaseContext.StockReservations
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM stock_reservations
                    WHERE order_id = {orderId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (reservation is null)
            {
                await transaction.CommitAsync(cancellationToken);
                return CancelReservationOperationResult.ReservationNotFound;
            }

            if (reservation.Status == StockReservationStatus.Canceled)
            {
                await transaction.CommitAsync(cancellationToken);
                return CancelReservationOperationResult.Success;
            }

            var reservationItems = await _databaseContext.StockReservationItems
                .Where(item => item.ReservationId == reservation.Id)
                .OrderBy(item => item.ProductId)
                .ToListAsync(cancellationToken);

            var productIds = reservationItems
                .Select(item => item.ProductId)
                .Distinct()
                .OrderBy(productId => productId)
                .ToArray();

            var stockItems = productIds.Length == 0
                ? []
                : await _databaseContext.StockItems
                    .FromSqlInterpolated($"""
                        SELECT *
                        FROM stock_items
                        WHERE product_id = ANY({productIds})
                        ORDER BY product_id
                        FOR UPDATE
                        """)
                    .ToListAsync(cancellationToken);

            var stockItemsByProductId = stockItems.ToDictionary(item => item.ProductId);
            var utcNow = DateTime.UtcNow;

            foreach (var reservationItem in reservationItems)
            {
                if (!stockItemsByProductId.TryGetValue(reservationItem.ProductId, out var stockItem))
                    continue;

                stockItem.CancelReservation(reservationItem.Quantity);

                await _databaseContext.StockMovements.AddAsync(
                    StockMovement.CreateReservationCanceled(
                        reservationItem.ProductId,
                        reservationItem.Quantity,
                        orderId,
                        utcNow),
                    cancellationToken);

                await _databaseContext.OutboxMessages.AddAsync(
                    StockChangedOutboxFactory.Create(
                        _kafkaOptions.WarehouseStockTopic,
                        stockItem,
                        utcNow),
                    cancellationToken);
            }

            reservation.Cancel();

            await _databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return CancelReservationOperationResult.Success;
        }

        public async Task<ReserveProductsOperationResult> ReserveAsync(
            Guid orderId,
            Guid userId,
            List<ReserveProductItem> reserveProductItems,
            CancellationToken cancellationToken)
        {
            if (reserveProductItems is null || reserveProductItems.Count == 0)
                return ReserveProductsOperationResult.InvalidItems();

            var requestedItems = reserveProductItems
                .GroupBy(item => item.ProductId)
                .Select(group => new ReserveProductItem(
                    group.Key,
                    group.Sum(item => item.Quantity)))
                .OrderBy(item => item.ProductId)
                .ToList();

            if (requestedItems.Any(item => item.ProductId == Guid.Empty || item.Quantity <= 0))
                return ReserveProductsOperationResult.InvalidItems();

            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            try
            {
                var existingReservation = await _databaseContext.StockReservations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        reservation => reservation.OrderId == orderId,
                        cancellationToken);

                if (existingReservation is not null)
                {
                    await transaction.CommitAsync(cancellationToken);

                    if (existingReservation.Status == StockReservationStatus.Reserved)
                        return ReserveProductsOperationResult.Success(existingReservation.Id);

                    return ReserveProductsOperationResult.InvalidReservationState();
                }

                var productIds = requestedItems
                    .Select(item => item.ProductId)
                    .ToArray();

                var stockItems = await _databaseContext.StockItems
                    .FromSqlInterpolated($"""
                        SELECT *
                        FROM stock_items
                        WHERE product_id = ANY({productIds})
                        ORDER BY product_id
                        FOR UPDATE
                        """)
                    .ToListAsync(cancellationToken);

                existingReservation = await _databaseContext.StockReservations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        reservation => reservation.OrderId == orderId,
                        cancellationToken);

                if (existingReservation is not null)
                {
                    await transaction.CommitAsync(cancellationToken);

                    if (existingReservation.Status == StockReservationStatus.Reserved)
                        return ReserveProductsOperationResult.Success(existingReservation.Id);

                    return ReserveProductsOperationResult.InvalidReservationState();
                }

                var stockItemsByProductId = stockItems.ToDictionary(item => item.ProductId);
                var unavailableItems = new List<UnavailableStockItem>();

                foreach (var requestedItem in requestedItems)
                {
                    if (!stockItemsByProductId.TryGetValue(requestedItem.ProductId, out var stockItem))
                    {
                        unavailableItems.Add(new UnavailableStockItem(
                            requestedItem.ProductId,
                            requestedItem.Quantity,
                            0));

                        continue;
                    }

                    if (!stockItem.IsActive)
                    {
                        unavailableItems.Add(new UnavailableStockItem(
                            stockItem.ProductId,
                            requestedItem.Quantity,
                            0));

                        continue;
                    }

                    if (stockItem.AvailableQuantity < requestedItem.Quantity)
                    {
                        unavailableItems.Add(new UnavailableStockItem(
                            stockItem.ProductId,
                            requestedItem.Quantity,
                            stockItem.AvailableQuantity));
                    }
                }

                if (unavailableItems.Count > 0)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ReserveProductsOperationResult.StockNotAvailable(unavailableItems);
                }

                var reservation = StockReservation.Create(orderId, userId);
                var utcNow = DateTime.UtcNow;

                var reservationItems = requestedItems
                    .Select(item => StockReservationItem.Create(
                        reservation.Id,
                        item.ProductId,
                        (uint)item.Quantity))
                    .ToList();

                foreach (var requestedItem in requestedItems)
                {
                    var stockItem = stockItemsByProductId[requestedItem.ProductId];
                    stockItem.Reserve(requestedItem.Quantity);

                    await _databaseContext.StockMovements.AddAsync(
                        StockMovement.CreateReservationCreated(
                            requestedItem.ProductId,
                            requestedItem.Quantity,
                            orderId,
                            utcNow),
                        cancellationToken);

                    await _databaseContext.OutboxMessages.AddAsync(
                        StockChangedOutboxFactory.Create(
                            _kafkaOptions.WarehouseStockTopic,
                            stockItem,
                            utcNow),
                        cancellationToken);
                }

                await _databaseContext.StockReservations.AddAsync(reservation, cancellationToken);
                await _databaseContext.StockReservationItems.AddRangeAsync(reservationItems, cancellationToken);

                await _databaseContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return ReserveProductsOperationResult.Success(reservation.Id);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException
                                               && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                await transaction.RollbackAsync(cancellationToken);

                _databaseContext.ChangeTracker.Clear();

                var reservation = await _databaseContext.StockReservations
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

                if (reservation is null)
                    throw;

                if (reservation.Status == StockReservationStatus.Canceled)
                    return ReserveProductsOperationResult.InvalidReservationState();

                return ReserveProductsOperationResult.Success(reservation.Id);
            }
        }
    }
}
