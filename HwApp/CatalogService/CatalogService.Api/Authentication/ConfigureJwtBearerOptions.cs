using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CatalogService.Api.Authentication
{
    internal sealed class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly JwksSigningKeyCache _jwksCache;
        private readonly IConfiguration _configuration;

        public ConfigureJwtBearerOptions(
            JwksSigningKeyCache jwksCache,
            IConfiguration configuration)
        {
            _jwksCache = jwksCache;
            _configuration = configuration;
        }

        public void PostConfigure(string? name, JwtBearerOptions options)
        {
            if (name is not null && name != JwtBearerDefaults.AuthenticationScheme)
                return;

            var authUrl = _configuration["Auth:Url"]?.Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(authUrl))
                throw new InvalidOperationException("Auth:Url must be configured.");

            options.RequireHttpsMetadata = false;
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authUrl,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (_, _, kid, _) => _jwksCache.GetIssuerSigningKeys(kid),
                NameClaimType = "sub",
                RoleClaimType = "role",
            };
        }
    }
}
