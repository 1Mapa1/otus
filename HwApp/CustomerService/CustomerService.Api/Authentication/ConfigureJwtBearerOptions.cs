using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CustomerService.Api.Authentication;

internal sealed class ConfigureJwtBearerOptions(
    JwksSigningKeyCache jwksCache,
    IConfiguration configuration)
    : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        if (name is not null && name != JwtBearerDefaults.AuthenticationScheme)
            return;

        var authUrl = configuration["Auth:Url"]?.Trim().TrimEnd('/');
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
            IssuerSigningKeyResolver = (_, _, kid, _) => jwksCache.GetIssuerSigningKeys(kid),
            NameClaimType = "sub",
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"AUTH FAILED: {context.Exception}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("TOKEN VALIDATED");
                Console.WriteLine($"sub = {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"AUTH CHALLENGE: {context.Error}; {context.ErrorDescription}");
                return Task.CompletedTask;
            },
        };
    }
}
