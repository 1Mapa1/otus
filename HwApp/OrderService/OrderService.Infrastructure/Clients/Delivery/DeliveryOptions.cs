namespace OrderService.Infrastructure.Clients.Delivery
{
    internal class DeliveryOptions
    {
        public const string SectionName = "Ms:Delivery";

        public string BaseUrl { get; set; } = null!;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
