using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace CatalogService.Infrastructure.Caching
{
    internal sealed class CatalogReferenceListCacheService : ICatalogReferenceListCacheService
    {
        private readonly CatalogDistributedCache _distributedCache;
        private readonly CatalogOptions _options;

        public CatalogReferenceListCacheService(
            CatalogDistributedCache distributedCache,
            IOptions<CatalogOptions> options)
        {
            _distributedCache = distributedCache;
            _options = options.Value;
        }

        public Task<IReadOnlyList<BrandListItemDto>> GetBrandsAsync(
            Func<CancellationToken, Task<IReadOnlyList<BrandListItemDto>>> replicaLoader,
            CancellationToken cancellationToken = default)
        {
            return _distributedCache.GetOrCreateWithLockAsync(
                CatalogCacheKeyBuilder.BrandsListKey,
                CatalogCacheKeyBuilder.BuildLockKey(CatalogCacheKeyBuilder.BrandsListKey),
                _options.BrandsCacheTtl,
                replicaLoader,
                cancellationToken);
        }

        public async Task RefreshBrandsAsync(
            Func<CancellationToken, Task<IReadOnlyList<BrandListItemDto>>> primaryLoader,
            CancellationToken cancellationToken = default)
        {
            var brands = await primaryLoader(cancellationToken);
            await _distributedCache.SetAsync(
                CatalogCacheKeyBuilder.BrandsListKey,
                brands,
                _options.BrandsCacheTtl,
                cancellationToken);
        }

        public Task<IReadOnlyList<CategoryListItemDto>> GetCategoriesAsync(
            Func<CancellationToken, Task<IReadOnlyList<CategoryListItemDto>>> replicaLoader,
            CancellationToken cancellationToken = default)
        {
            return _distributedCache.GetOrCreateWithLockAsync(
                CatalogCacheKeyBuilder.CategoriesListKey,
                CatalogCacheKeyBuilder.BuildLockKey(CatalogCacheKeyBuilder.CategoriesListKey),
                _options.CategoriesCacheTtl,
                replicaLoader,
                cancellationToken);
        }

        public async Task RefreshCategoriesAsync(
            Func<CancellationToken, Task<IReadOnlyList<CategoryListItemDto>>> primaryLoader,
            CancellationToken cancellationToken = default)
        {
            var categories = await primaryLoader(cancellationToken);
            await _distributedCache.SetAsync(
                CatalogCacheKeyBuilder.CategoriesListKey,
                categories,
                _options.CategoriesCacheTtl,
                cancellationToken);
        }
    }
}
