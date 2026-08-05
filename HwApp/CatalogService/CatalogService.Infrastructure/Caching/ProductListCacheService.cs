using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Infrastructure.Caching
{
    internal sealed class ProductListCacheService : IProductListCacheService
    {
        private readonly CatalogDistributedCache _distributedCache;

        public ProductListCacheService(CatalogDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public Task<ProductListResultDto> GetOrLoadAsync(
            ProductListQuery query,
            Func<CancellationToken, Task<ProductListResultDto>> loader,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = CatalogCacheKeyBuilder.BuildListKey(query);
            var lockKey = CatalogCacheKeyBuilder.BuildListLockKey(query);
            var ttl = TimeSpan.FromSeconds(60 + Random.Shared.Next(0, 16));

            return _distributedCache.GetOrCreateWithLockAsync(
                cacheKey,
                lockKey,
                ttl,
                loader,
                cancellationToken);
        }
    }
}
