namespace OrderService.Infrastructure.Options
{
    internal sealed class CatalogOptions
    {
        public const string SectionName = "Ms:Catalog";

        public string BaseUrl { get; set; } = null!;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
