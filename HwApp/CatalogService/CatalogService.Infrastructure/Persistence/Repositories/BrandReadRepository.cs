using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class BrandReadRepository : IBrandReadRepository
    {
        private readonly CatalogReadDbContext _context;

        public BrandReadRepository(CatalogReadDbContext context)
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
