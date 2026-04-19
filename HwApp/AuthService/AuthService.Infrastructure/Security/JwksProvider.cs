using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security
{
    internal class JwksProvider : IJwksProvider
    {
        private readonly RsaJwtSigningKeyProvider _rsaKeyProvider;

        public JwksProvider(RsaJwtSigningKeyProvider rsaKeyProvider)
        {
            _rsaKeyProvider = rsaKeyProvider;
        }

        public object Get()
        {
            var key = _rsaKeyProvider.GetSecuritgKey();

            var rsa = key.Rsa;

            var parameters = rsa.ExportParameters(false);

            var n = Base64UrlEncoder.Encode(parameters.Modulus);
            var e = Base64UrlEncoder.Encode(parameters.Exponent);

            return new
            {
                keys = new[]
                {
                    new
                    {
                        kty = "RSA",
                        use = "sig",
                        kid = key.KeyId,
                        alg = "RS256",
                        n,
                        e
                    }
                }
            };
        }
    }
}
