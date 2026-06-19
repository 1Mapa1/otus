using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;

namespace WarehouseService.Api.Authentication
{
    internal sealed class JwksSigningKeyCache
    {
        public const string HttpClientName = "Jwks";

        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<JwksSigningKeyCache> _logger;
        private readonly string _jwksUrl;
        private readonly object _gate = new();
        private JsonWebKeySet? _jwks;
        private DateTime _expiresUtc;
        private readonly ConcurrentDictionary<string, byte> _loggedKids = new();

        public JwksSigningKeyCache(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<JwksSigningKeyCache> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            var authUrl = configuration["Auth:Url"]?.Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(authUrl))
                throw new InvalidOperationException("Auth:Url must be configured.");

            _jwksUrl = $"{authUrl}/.well-known/jwks.json";
        }

        public IEnumerable<SecurityKey> GetIssuerSigningKeys(string? kid)
        {
            var keys = LoadJwks(forceRefresh: false).ToList();

            if (string.IsNullOrEmpty(kid))
                return keys;

            var resolved = keys.FirstOrDefault(k =>
                string.Equals(k.KeyId, kid, StringComparison.OrdinalIgnoreCase));

            if (resolved is not null)
                return new[] { resolved };

            _logger.LogInformation("JWKS kid {Kid} not in cache; forcing refresh.", kid);
            keys = LoadJwks(forceRefresh: true).ToList();
            resolved = keys.FirstOrDefault(k =>
                string.Equals(k.KeyId, kid, StringComparison.OrdinalIgnoreCase));

            if (resolved is not null)
                return new[] { resolved };

            if (_loggedKids.TryAdd(kid, 0))
                _logger.LogWarning("No signing key found for kid {Kid}.", kid);

            return Array.Empty<SecurityKey>();
        }

        private IEnumerable<SecurityKey> LoadJwks(bool forceRefresh)
        {
            lock (_gate)
            {
                if (!forceRefresh && _jwks is not null && DateTime.UtcNow < _expiresUtc)
                    return _jwks.GetSigningKeys();

                var client = _httpClientFactory.CreateClient(HttpClientName);
                _logger.LogInformation("Fetching JWKS from {JwksUrl}", _jwksUrl);
                var json = client.GetStringAsync(_jwksUrl).GetAwaiter().GetResult();
                _jwks = new JsonWebKeySet(json);
                _expiresUtc = DateTime.UtcNow.Add(CacheTtl);
                _logger.LogInformation("JWKS loaded; key count = {Count}", _jwks.Keys.Count);
                return _jwks.GetSigningKeys();
            }
        }
    }
}
