using System.Text.Json;
using CatalogService.Application.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CatalogService.Infrastructure.Caching
{
    internal sealed class CatalogDistributedCache
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        private static readonly TimeSpan DefaultLockExpiry = TimeSpan.FromSeconds(5);
        private static readonly int[] DefaultWaitDelaysMs = [50, 100, 200];

        private readonly IDistributedCache _cache;
        private readonly RedisConnectionAccessor _redisConnectionAccessor;
        private readonly ILogger<CatalogDistributedCache> _logger;

        public CatalogDistributedCache(
            IDistributedCache cache,
            RedisConnectionAccessor redisConnectionAccessor,
            ILogger<CatalogDistributedCache> logger)
        {
            _cache = cache;
            _redisConnectionAccessor = redisConnectionAccessor;
            _logger = logger;
        }

        public async Task<T> GetOrCreateWithLockAsync<T>(
            string cacheKey,
            string lockKey,
            TimeSpan ttl,
            Func<CancellationToken, Task<T>> loader,
            CancellationToken cancellationToken = default,
            TimeSpan? lockExpiry = null,
            int[]? waitDelaysMs = null)
        {
            if (!IsRedisEnabled())
            {
                _logger.LogWarning("Redis is unavailable. Bypassing cache for key {CacheKey}.", cacheKey);
                return await loader(cancellationToken);
            }

            var cached = await GetAsync<T>(cacheKey, cancellationToken);
            if (cached is not null)
                return cached;

            var database = _redisConnectionAccessor.Connection!.GetDatabase();
            var delays = waitDelaysMs ?? DefaultWaitDelaysMs;
            var lockTtl = lockExpiry ?? DefaultLockExpiry;

            for (var attempt = 0; attempt <= delays.Length; attempt++)
            {
                await using var cacheLock = await RedisCacheLock.TryAcquireAsync(
                    database,
                    lockKey,
                    lockTtl,
                    cancellationToken);

                if (cacheLock.IsAcquired)
                {
                    cached = await GetAsync<T>(cacheKey, cancellationToken);
                    if (cached is not null)
                        return cached;

                    var loaded = await loader(cancellationToken);
                    await SetAsync(cacheKey, loaded, ttl, cancellationToken);
                    return loaded;
                }

                if (attempt < delays.Length)
                {
                    await Task.Delay(delays[attempt], cancellationToken);
                    cached = await GetAsync<T>(cacheKey, cancellationToken);
                    if (cached is not null)
                        return cached;
                }
            }

            throw new ServiceUnavailableException();
        }

        public async Task SetAsync<T>(
            string cacheKey,
            T value,
            TimeSpan ttl,
            CancellationToken cancellationToken = default)
        {
            if (!IsRedisEnabled())
            {
                _logger.LogWarning("Redis is unavailable. Skipping cache write for key {CacheKey}.", cacheKey);
                return;
            }

            try
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
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
                _logger.LogWarning(ex, "Failed to write cache entry for key {CacheKey}.", cacheKey);
            }
        }

        private bool IsRedisEnabled()
            => _redisConnectionAccessor.Connection is not null && _redisConnectionAccessor.Connection.IsConnected;

        private async Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken)
        {
            if (!IsRedisEnabled())
                return default;

            try
            {
                var bytes = await _cache.GetAsync(cacheKey, cancellationToken);
                if (bytes is null || bytes.Length == 0)
                    return default;

                return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read cache entry for key {CacheKey}.", cacheKey);
                return default;
            }
        }
    }
}
