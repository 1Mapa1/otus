using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Domain.Brands;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence.Repositories
{
    internal sealed class BrandWriteRepository : IBrandWriteRepository
    {
        private readonly CatalogWriteDbContext _context;

        public BrandWriteRepository(CatalogWriteDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Brand brand, CancellationToken cancellationToken = default)
        {
            return _context.Brands.AddAsync(brand, cancellationToken).AsTask();
        }

        public Task<Brand?> GetByIdAsync(Guid brandId, CancellationToken cancellationToken = default)
        {
            return _context.Brands.FirstOrDefaultAsync(brand => brand.Id == brandId, cancellationToken);
        }

        public Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeBrandId = null,
            CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim();

            return _context.Brands.AsNoTracking()
                .AnyAsync(
                    brand => brand.Name == normalized && (excludeBrandId == null || brand.Id != excludeBrandId),
                    cancellationToken);
        }
    }
}
