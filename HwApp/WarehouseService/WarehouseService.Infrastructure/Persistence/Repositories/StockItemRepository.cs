using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WarehouseService.Application.Stocks;
using WarehouseService.Domain.Stocks;
using WarehouseService.Infrastructure.Messaging;
using WarehouseService.Infrastructure.Messaging.Kafka;
using WarehouseService.Infrastructure.Persistence.Outbox;

namespace WarehouseService.Infrastructure.Persistence.Repositories
{
    internal sealed class StockItemRepository : IStockItemRepository
    {
        private readonly DatabaseContext _databaseContext;
        private readonly KafkaOptions _kafkaOptions;

        public StockItemRepository(
            DatabaseContext databaseContext,
            IOptions<KafkaOptions> kafkaOptions)
        {
            _databaseContext = databaseContext;
            _kafkaOptions = kafkaOptions.Value;
        }

        public async Task AddAsync(StockItem stockItem, CancellationToken cancellationToken)
        {
            await _databaseContext.StockItems.AddAsync(stockItem, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid productId, CancellationToken cancellationToken)
        {
            return await _databaseContext.StockItems
                .AsNoTracking()
                .AnyAsync(item => item.ProductId == productId, cancellationToken);
        }

        public async Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.StockItems
                .AsNoTracking()
                .OrderBy(item => item.ProductId)
                .ToListAsync(cancellationToken);
        }

        public async Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            return await _databaseContext.StockItems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.ProductId == productId, cancellationToken);
        }

        public async Task<StockIncomeOperationResult> IncomeAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var stockItem = await _databaseContext.StockItems
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM stock_items
                    WHERE product_id = {productId}
                    FOR UPDATE
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (stockItem is null)
            {
                await transaction.CommitAsync(cancellationToken);
                return StockIncomeOperationResult.StockItemNotFound;
            }

            if (!stockItem.IsActive)
            {
                await transaction.RollbackAsync(cancellationToken);
                return StockIncomeOperationResult.Conflict;
            }

            var utcNow = DateTime.UtcNow;

            stockItem.IncreaseAvailableQuantity(quantity);

            var movement = StockMovement.CreateIncome(productId, quantity, utcNow);

            await _databaseContext.StockMovements.AddAsync(movement, cancellationToken);
            await _databaseContext.OutboxMessages.AddAsync(
                StockChangedOutboxFactory.Create(
                    _kafkaOptions.WarehouseStockTopic,
                    stockItem,
                    utcNow),
                cancellationToken);

            await _databaseContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return StockIncomeOperationResult.Success;
        }
    }
}
