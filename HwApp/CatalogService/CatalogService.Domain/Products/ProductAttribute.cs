namespace CatalogService.Domain.Products
{
    public sealed class ProductAttribute
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }

        public string Name { get; private set; } = null!;

        public string Value { get; private set; } = null!;

        public int SortOrder { get; private set; }

        private ProductAttribute()
        {
        }

        internal static ProductAttribute Create(
            Guid productId,
            string name,
            string value,
            int sortOrder)
        {
            return new ProductAttribute
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Name = name.Trim(),
                Value = value.Trim(),
                SortOrder = sortOrder
            };
        }
    }
}
