using StackExchange.Redis;

namespace CatalogService.Infrastructure.Caching
{
    internal sealed class RedisConnectionAccessor
    {
        public RedisConnectionAccessor(IConnectionMultiplexer? connection)
        {
            Connection = connection;
        }

        public IConnectionMultiplexer? Connection { get; }
    }
}
