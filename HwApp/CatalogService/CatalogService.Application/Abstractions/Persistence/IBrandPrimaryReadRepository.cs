using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface IBrandPrimaryReadRepository
    {
        Task<IReadOnlyList<BrandListItemDto>> GetBrandsAsync(
            CancellationToken cancellationToken = default);
    }
}
