using AuthService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    public class WellKnownController : ControllerBase
    {
        private readonly IJwksProvider _jwksProvider;
        private readonly IConfiguration _configuration;

        public WellKnownController(IJwksProvider jwksProvider, IConfiguration configuration)
        {
            _jwksProvider = jwksProvider;
            _configuration = configuration;
        }

        [HttpGet("/.well-known/jwks.json")]
        public IActionResult GetJwks()
        {
            var jwks = _jwksProvider.Get();
            return Ok(jwks);
        }

        [HttpGet("/.well-known/openid-configuration")]
        public IActionResult GetOpenIdConfiguration()
        {
            var issuerBase = ResolveIssuerBaseUrl();

            return Ok(new
            {
                issuer = issuerBase,
                jwks_uri = $"{issuerBase}/.well-known/jwks.json",
                authorization_endpoint = $"{issuerBase}/connect/authorize",
                token_endpoint = $"{issuerBase}/connect/token",
                userinfo_endpoint = $"{issuerBase}/connect/userinfo",
                end_session_endpoint = $"{issuerBase}/connect/endsession",
                response_types_supported = new[] { "token" },
                subject_types_supported = new[] { "public" },
                id_token_signing_alg_values_supported = new[] { "RS256" },
                claims_supported = new[] { "sub", "iss", "exp", "nbf" }
            });
        }

        private string ResolveIssuerBaseUrl()
        {
            var issuer = _configuration["Jwt:Issuer"];

            if (string.IsNullOrWhiteSpace(issuer))
            {
                issuer = $"{Request.Scheme}://{Request.Host}";
            }

            return issuer.TrimEnd('/');
        }
    }
}
