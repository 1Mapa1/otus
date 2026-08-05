using System.Text.Json;

namespace CatalogService.Application.Products
{
    public static class ProductAttributeJsonBuilder
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

        public static string BuildValues(IReadOnlyList<(string Name, string Value)> attributes)
        {
            var values = attributes.Select(attribute => attribute.Value).ToList();
            return JsonSerializer.Serialize(values, Options);
        }
    }
}
