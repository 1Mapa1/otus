using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Application.Abstractions.Persistence
{
    public interface IBrandReadRepository
    {
        Task<IReadOnlyList<BrandListItemDto>> GetBrandsAsync(
            CancellationToken cancellationToken = default);
    }
}
