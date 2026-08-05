using StackExchange.Redis;

namespace CatalogService.Infrastructure.Caching
{
    internal sealed class RedisCacheLock : IAsyncDisposable
    {
        private const string ReleaseScript =
            """
            if redis.call('GET', KEYS[1]) == ARGV[1] then
                return redis.call('DEL', KEYS[1])
            end

            return 0
            """;

        private readonly IDatabase _database;
        private readonly string _lockKey;
        private readonly string _token;
        private bool _acquired;
        private bool _disposed;

        private RedisCacheLock(IDatabase database, string lockKey, string token, bool acquired)
        {
            _database = database;
            _lockKey = lockKey;
            _token = token;
            _acquired = acquired;
        }

        public bool IsAcquired => _acquired;

        public static async Task<RedisCacheLock> TryAcquireAsync(
            IDatabase database,
            string lockKey,
            TimeSpan expiry,
            CancellationToken cancellationToken = default)
        {
            var token = Guid.NewGuid().ToString("N");
            var acquired = await database.StringSetAsync(lockKey, token, expiry, When.NotExists);

            return new RedisCacheLock(database, lockKey, token, acquired);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed || !_acquired)
                return;

            _disposed = true;
            _acquired = false;

            await _database.ScriptEvaluateAsync(
                ReleaseScript,
                new RedisKey[] { _lockKey },
                new RedisValue[] { _token });
        }
    }
}
