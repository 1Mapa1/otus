namespace OrderService.Infrastructure.Clients.Billing
{
    internal class BillingOptions
    {
        public const string SectionName = "Ms:Billing";

        public string BaseUrl { get; set; } = null!;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
