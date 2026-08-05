using WarehouseService.Domain.Stocks;

namespace WarehouseService.Application.Stocks
{
    public interface IStockItemRepository
    {
        Task AddAsync(StockItem stockItem, CancellationToken cancellationToken);

        Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken);

        Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(Guid productId, CancellationToken cancellationToken);

        Task<StockIncomeOperationResult> IncomeAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken);
    }
}
