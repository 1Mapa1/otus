using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class BrandPrimaryReadRepository : IBrandPrimaryReadRepository
    {
        private readonly CatalogWriteDbContext _context;

        public BrandPrimaryReadRepository(CatalogWriteDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BrandListItemDto>> GetBrandsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Brands
                .AsNoTracking()
                .OrderBy(brand => brand.Name)
                .Select(brand => new BrandListItemDto(brand.Id, brand.Name))
                .ToListAsync(cancellationToken);
        }
    }
}
