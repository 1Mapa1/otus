using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Application.Abstractions.Caching
{
    public interface ICatalogReferenceListCacheService
    {
        Task<IReadOnlyList<BrandListItemDto>> GetBrandsAsync(
            Func<CancellationToken, Task<IReadOnlyList<BrandListItemDto>>> replicaLoader,
            CancellationToken cancellationToken = default);

        Task RefreshBrandsAsync(
            Func<CancellationToken, Task<IReadOnlyList<BrandListItemDto>>> primaryLoader,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CategoryListItemDto>> GetCategoriesAsync(
            Func<CancellationToken, Task<IReadOnlyList<CategoryListItemDto>>> replicaLoader,
            CancellationToken cancellationToken = default);

        Task RefreshCategoriesAsync(
            Func<CancellationToken, Task<IReadOnlyList<CategoryListItemDto>>> primaryLoader,
            CancellationToken cancellationToken = default);
    }
}
