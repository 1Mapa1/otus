namespace AuthService.Infrastructure.Security
{
    internal sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; set; } = default!;

        public int LifetimeMinutes { get; set; }

        public string PrivateKeyPem { get; set; } = default!;

        public string KeyId { get; set; } = default!;
    }
}
