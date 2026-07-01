using CatalogService.Domain.Brands;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface IBrandWriteRepository
    {
        Task AddAsync(Brand brand, CancellationToken cancellationToken = default);

        Task<Brand?> GetByIdAsync(Guid brandId, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeBrandId = null,
            CancellationToken cancellationToken = default);
    }
}
