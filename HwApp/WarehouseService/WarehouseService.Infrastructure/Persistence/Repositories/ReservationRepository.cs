using Microsoft.EntityFrameworkCore;
using WarehouseService.Application.Reservations;
using WarehouseService.Application.Reservations.Operations;
using WarehouseService.Domain.StockReservations;

namespace WarehouseService.Infrastructure.Persistence.Repositories
{
    internal sealed class ReservationRepository : IReservationRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ReservationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<CancelReservationOperationResult> CancelAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var reservation = await _databaseContext.StockReservations
                .FirstOrDefaultAsync(
                    stockReservation => stockReservation.OrderId == orderId,
                    cancellationToken);

            if (reservation is null)
            {
                await transaction.CommitAsync(cancellationToken);
                return CancelReservationOperationResult.Success;
            }

            if (reservation.Status == StockReservationStatus.Canceled)
            {
                await transaction.CommitAsync(cancellationToken);
                return CancelReservationOperationResult.Success;
            }

            var reservationItems = await _databaseContext.StockReservationItems
                .Where(item => item.ReservationId == reservation.Id)
                .ToListAsync(cancellationToken);

            var productIds = reservationItems
               .Select(item => item.ProductId)
               .Distinct()
               .OrderBy(productId => productId)
               .ToArray();

            var products = await _databaseContext.Products
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM products
                    WHERE id = ANY({productIds})
                    ORDER BY id
                    FOR UPDATE
                    """)
                .ToListAsync(cancellationToken);

            var productsById = products.ToDictionary(product => product.Id);

            foreach (var reservationItem in reservationItems)
            {
                if (!productsById.TryGetValue(reservationItem.ProductId, out var product))
                    continue;

                product.DecreaseReservedQuantity((uint)reservationItem.Quantity);
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

            var existingReservation = await _databaseContext.StockReservations
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

            var products = await _databaseContext.Products
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM products
                    WHERE id = ANY({productIds})
                    ORDER BY id
                    FOR UPDATE
                """)
                .ToListAsync(cancellationToken);

            var productsById = products.ToDictionary(product => product.Id);

            var unavailableItems = new List<UnavailableStockItem>();

            foreach (var requestedItem in requestedItems)
            {
                if (!productsById.TryGetValue(requestedItem.ProductId, out var product))
                {
                    unavailableItems.Add(new UnavailableStockItem(
                        requestedItem.ProductId,
                        requestedItem.Quantity,
                        0));

                    continue;
                }

                var currentAvailableQuantity = product.AvailableQuantity - product.ReservedQuantity;

                if (currentAvailableQuantity < requestedItem.Quantity)
                {
                    unavailableItems.Add(new UnavailableStockItem(
                        product.Id,
                        requestedItem.Quantity,
                        currentAvailableQuantity));
                }
            }

            if (unavailableItems.Count > 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ReserveProductsOperationResult.StockNotAvailable(unavailableItems);
            }

            var reservation = StockReservation.Create(
                orderId,
                userId);

            var reservationItems = requestedItems
                .Select(item => StockReservationItem.Create(
                    reservation.Id,
                    item.ProductId,
                    (uint)item.Quantity))
                .ToList();

            foreach (var requestedItem in requestedItems)
            {
                var product = productsById[requestedItem.ProductId];
                product.IncreaseReservedQuantity((uint)requestedItem.Quantity);
            }

            await _databaseContext.StockReservations.AddAsync(reservation, cancellationToken);
            await _databaseContext.StockReservationItems.AddRangeAsync(reservationItems, cancellationToken);

            await _databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return ReserveProductsOperationResult.Success(reservation.Id);
        }
    }
}
