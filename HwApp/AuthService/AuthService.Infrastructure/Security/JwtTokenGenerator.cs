using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Infrastructure.Security
{
    internal class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _options;
        private readonly RsaJwtSigningKeyProvider _keyProvider;

        public JwtTokenGenerator(
            IOptions<JwtOptions> options,
            RsaJwtSigningKeyProvider keyProvider)
        {
            _options = options.Value;
            _keyProvider = keyProvider;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_options.LifetimeMinutes),
                signingCredentials: _keyProvider.GetSigningKey());

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}