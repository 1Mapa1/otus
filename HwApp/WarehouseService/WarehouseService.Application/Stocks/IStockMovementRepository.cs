using WarehouseService.Domain.Stocks;

namespace WarehouseService.Application.Stocks
{
    public interface IStockMovementRepository
    {
        Task<IReadOnlyList<StockMovement>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken);
    }
}
