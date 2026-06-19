namespace OrderService.Infrastructure.Options
{
    internal class WarehouseOptions
    {
        public const string SectionName = "Ms:Warehouse";

        public string BaseUrl { get; set; } = null!;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
