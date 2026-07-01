using System.Text.Json;
using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Queries;
using CatalogService.Application.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CatalogService.Infrastructure.Caching
{
    internal sealed class ProductListCacheService : IProductListCacheService
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private static readonly TimeSpan LockExpiry = TimeSpan.FromSeconds(5);
        private static readonly int[] LoserDelaysMs = [50, 100, 200];

        private readonly IDistributedCache _cache;
        private readonly RedisConnectionAccessor _redisConnectionAccessor;
        private readonly ILogger<ProductListCacheService> _logger;

        public ProductListCacheService(
            IDistributedCache cache,
            RedisConnectionAccessor redisConnectionAccessor,
            ILogger<ProductListCacheService> logger)
        {
            _cache = cache;
            _redisConnectionAccessor = redisConnectionAccessor;
            _logger = logger;
        }

        public async Task<ProductListResultDto> GetOrLoadAsync(
            ProductListQuery query,
            Func<CancellationToken, Task<ProductListResultDto>> loader,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = CatalogCacheKeyBuilder.BuildListKey(query);

            if (!IsRedisEnabled())
            {
                _logger.LogWarning("Redis is unavailable. Bypassing cache for product list.");
                return await loader(cancellationToken);
            }

            var cached = await GetCachedAsync(cacheKey, cancellationToken);
            if (cached is not null)
                return cached;

            var lockKey = CatalogCacheKeyBuilder.BuildListLockKey(query);
            var database = _redisConnectionAccessor.Connection!.GetDatabase();

            for (var attempt = 0; attempt <= LoserDelaysMs.Length; attempt++)
            {
                await using var cacheLock = await RedisCacheLock.TryAcquireAsync(
                    database,
                    lockKey,
                    LockExpiry,
                    cancellationToken);

                if (cacheLock.IsAcquired)
                {
                    cached = await GetCachedAsync(cacheKey, cancellationToken);
                    if (cached is not null)
                        return cached;

                    var loaded = await loader(cancellationToken);
                    await SetCachedAsync(cacheKey, loaded, cancellationToken);
                    return loaded;
                }

                if (attempt < LoserDelaysMs.Length)
                {
                    await Task.Delay(LoserDelaysMs[attempt], cancellationToken);
                    cached = await GetCachedAsync(cacheKey, cancellationToken);
                    if (cached is not null)
                        return cached;
                }
            }

            throw new ServiceUnavailableException();
        }

        private bool IsRedisEnabled()
            => _redisConnectionAccessor.Connection is not null && _redisConnectionAccessor.Connection.IsConnected;

        private async Task<ProductListResultDto?> GetCachedAsync(string cacheKey, CancellationToken cancellationToken)
        {
            if (!IsRedisEnabled())
                return null;

            try
            {
                var bytes = await _cache.GetAsync(cacheKey, cancellationToken);
                if (bytes is null || bytes.Length == 0)
                    return null;

                return JsonSerializer.Deserialize<ProductListResultDto>(bytes, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read product list cache entry.");
                return null;
            }
        }

        private async Task SetCachedAsync(
            string cacheKey,
            ProductListResultDto value,
            CancellationToken cancellationToken)
        {
            if (!IsRedisEnabled())
                return;

            try
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
                var ttl = TimeSpan.FromSeconds(60 + Random.Shared.Next(0, 16));

                await _cache.SetAsync(
                    cacheKey,
                    bytes,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write product list cache entry.");
            }
        }
    }
}
