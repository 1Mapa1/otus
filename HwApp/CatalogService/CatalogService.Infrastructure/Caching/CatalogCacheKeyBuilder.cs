using CatalogService.Application.Abstractions.Queries;

namespace CatalogService.Infrastructure.Caching
{
    internal static class CatalogCacheKeyBuilder
    {
        public static string BuildListKey(ProductListQuery query)
        {
            var search = NormalizeSearch(query.Search);
            var brand = query.BrandId?.ToString("D") ?? "all";
            var category = query.CategoryId?.ToString("D") ?? "all";
            var min = query.MinPrice?.ToString("0.00") ?? "none";
            var max = query.MaxPrice?.ToString("0.00") ?? "none";

            return $"products:list:fmt1:brand:{brand}:category:{category}:search:{search}:min:{min}:max:{max}:sort:{query.Sort}:page:{query.Page}:size:{query.PageSize}";
        }

        public static string BuildListLockKey(ProductListQuery query)
            => $"catalog:locks:{BuildListKey(query)}";

        private static string NormalizeSearch(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return "all";

            return search.Trim().ToLowerInvariant();
        }
    }
}
