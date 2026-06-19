using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WarehouseService.Application.Products;
using WarehouseService.Domain.Products;

namespace WarehouseService.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ProductRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            await _databaseContext.Products.AddAsync(product, cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.Products.ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken)
        {
            var ids = productIds.Distinct().ToArray();

            if (ids.Length == 0)
                return [];

            return await _databaseContext.Products
                .Where(product => ids.Contains(product.Id))
                .ToListAsync(cancellationToken);
        }
    }
}
