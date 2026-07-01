namespace CatalogService.Domain.Products
{
    public sealed class ProductReadModel
    {
        public Guid ProductId { get; private set; }

        public string Name { get; private set; } = null!;

        public decimal Price { get; private set; }

        public string ImageUrl { get; private set; } = null!;

        public Guid BrandId { get; private set; }

        public string BrandName { get; private set; } = null!;

        public Guid CategoryId { get; private set; }

        public string AttributeValuesJson { get; private set; } = "[]";

        public AvailabilityStatus AvailabilityStatus { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private ProductReadModel()
        {
        }

        public static ProductReadModel CreateForProduct(
            Product product,
            string brandName,
            string attributeValuesJson,
            DateTime utcNow)
        {
            return new ProductReadModel
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                BrandId = product.BrandId,
                BrandName = brandName,
                CategoryId = product.CategoryId,
                AttributeValuesJson = attributeValuesJson,
                AvailabilityStatus = AvailabilityStatus.OutOfStock,
                IsActive = product.IsActive,
                UpdatedAt = utcNow
            };
        }

        public void SyncFromProduct(
            Product product,
            string brandName,
            string attributeValuesJson,
            DateTime utcNow)
        {
            Name = product.Name;
            Price = product.Price;
            ImageUrl = product.ImageUrl;
            BrandId = product.BrandId;
            BrandName = brandName;
            CategoryId = product.CategoryId;
            AttributeValuesJson = attributeValuesJson;
            IsActive = product.IsActive;
            UpdatedAt = utcNow;
        }

        public void SetAvailability(AvailabilityStatus status, DateTime utcNow)
        {
            AvailabilityStatus = status;
            UpdatedAt = utcNow;
        }
    }
}
