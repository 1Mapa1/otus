namespace OrderService.Infrastructure.Billing
{
    internal sealed class BillingServiceOptions
    {
        public const string SectionName = "Ms:Billing";

        public string BaseUrl { get; set; } = null!;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
