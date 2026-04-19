using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AuthService.Infrastructure.Security
{
    internal class RsaJwtSigningKeyProvider
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly RsaSecurityKey _key;

        public RsaJwtSigningKeyProvider(IOptions<JwtOptions> options)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(options.Value.PrivateKeyPem.ToCharArray());

            _key = new RsaSecurityKey(rsa)
            {
                KeyId = options.Value.KeyId
            };

            _signingCredentials = new SigningCredentials(
                _key,
                SecurityAlgorithms.RsaSha256);
        }

        public SigningCredentials GetSigningKey() => _signingCredentials;
        public RsaSecurityKey GetSecuritgKey() => _key;
    }
}
