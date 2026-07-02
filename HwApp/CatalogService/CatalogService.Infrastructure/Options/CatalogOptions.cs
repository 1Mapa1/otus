namespace CatalogService.Infrastructure.Options
{
    internal sealed class CatalogOptions
    {
        public const string SectionName = "Catalog";

        public int LowStockThreshold { get; init; } = 5;

        public TimeSpan BrandsCacheTtl { get; init; } = TimeSpan.FromMinutes(30);

        public TimeSpan CategoriesCacheTtl { get; init; } = TimeSpan.FromMinutes(30);
    }
}
