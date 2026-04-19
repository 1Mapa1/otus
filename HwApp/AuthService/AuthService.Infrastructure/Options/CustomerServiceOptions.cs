namespace AuthService.Infrastructure.Options
{
    internal sealed class CustomerServiceOptions
    {
        public const string SectionName = "Ms:Customer";

        public string BaseUrl { get; set; } = null!;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
