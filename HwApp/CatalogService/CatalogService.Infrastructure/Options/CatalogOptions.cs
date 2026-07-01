namespace CatalogService.Infrastructure.Options
{
    internal sealed class CatalogOptions
    {
        public const string SectionName = "Catalog";

        public int LowStockThreshold { get; init; } = 5;
    }
}
