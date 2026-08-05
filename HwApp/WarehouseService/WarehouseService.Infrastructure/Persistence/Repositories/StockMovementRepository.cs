using Microsoft.EntityFrameworkCore;
using WarehouseService.Application.Stocks;
using WarehouseService.Domain.Stocks;

namespace WarehouseService.Infrastructure.Persistence.Repositories
{
    internal sealed class StockMovementRepository : IStockMovementRepository
    {
        private readonly DatabaseContext _databaseContext;

        public StockMovementRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IReadOnlyList<StockMovement>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return await _databaseContext.StockMovements
                .AsNoTracking()
                .Where(movement => movement.ProductId == productId)
                .OrderByDescending(movement => movement.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
