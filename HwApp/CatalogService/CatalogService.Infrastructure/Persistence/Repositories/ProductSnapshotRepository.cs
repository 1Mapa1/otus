using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Domain.Products;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductSnapshotRepository : IProductSnapshotRepository
    {
        private readonly CatalogWriteDbContext _context;

        public ProductSnapshotRepository(CatalogWriteDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ProductSnapshotRow>> GetActiveProductsAsync(
            IReadOnlyList<Guid> productIds,
            CancellationToken cancellationToken = default)
        {
            if (productIds.Count == 0)
                return [];

            return await _context.Products
                .AsNoTracking()
                .Where(product => productIds.Contains(product.Id))
                .Select(product => new ProductSnapshotRow(
                    product.Id,
                    product.Name,
                    product.Price,
                    product.IsActive))
                .ToListAsync(cancellationToken);
        }
    }
}
