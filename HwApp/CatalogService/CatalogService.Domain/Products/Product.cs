using CatalogService.Domain.Events;
using CatalogService.Domain.Products.Events;

namespace CatalogService.Domain.Products
{
    public sealed class Product : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _events = [];
        private readonly List<ProductAttribute> _attributes = [];

        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public string Description { get; private set; } = null!;

        public Guid BrandId { get; private set; }

        public Guid CategoryId { get; private set; }

        public decimal Price { get; private set; }

        public string ImageUrl { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes.AsReadOnly();

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        private Product()
        {
        }

        public static Product Create(
            string name,
            string description,
            Guid brandId,
            Guid categoryId,
            decimal price,
            string imageUrl,
            IReadOnlyList<(string Name, string Value)> attributes,
            DateTime utcNow)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Description = description.Trim(),
                BrandId = brandId,
                CategoryId = categoryId,
                Price = price,
                ImageUrl = imageUrl.Trim(),
                IsActive = true,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            product.ReplaceAttributes(attributes);
            product._events.Add(new ProductCreatedEvent(product.Id));
            return product;
        }

        public void Update(
            string name,
            string description,
            Guid brandId,
            Guid categoryId,
            decimal price,
            string imageUrl,
            IReadOnlyList<(string Name, string Value)> attributes,
            DateTime utcNow)
        {
            Name = name.Trim();
            Description = description.Trim();
            BrandId = brandId;
            CategoryId = categoryId;
            Price = price;
            ImageUrl = imageUrl.Trim();
            UpdatedAt = utcNow;
            ReplaceAttributes(attributes);
        }

        public void Archive(DateTime utcNow)
        {
            IsActive = false;
            UpdatedAt = utcNow;
            _events.Add(new ProductArchivedEvent(Id));
        }

        public void Restore(DateTime utcNow)
        {
            IsActive = true;
            UpdatedAt = utcNow;
            _events.Add(new ProductRestoredEvent(Id));
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        private void ReplaceAttributes(IReadOnlyList<(string Name, string Value)> attributes)
        {
            _attributes.Clear();

            for (var index = 0; index < attributes.Count; index++)
            {
                var attribute = attributes[index];
                _attributes.Add(ProductAttribute.Create(
                    Id,
                    attribute.Name,
                    attribute.Value,
                    index + 1));
            }
        }
    }
}
