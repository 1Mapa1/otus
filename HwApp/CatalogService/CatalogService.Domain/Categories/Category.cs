namespace CatalogService.Domain.Categories
{
    public sealed class Category
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private Category()
        {
        }

        public static Category Create(string name, DateTime utcNow)
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
        }

        public void Rename(string name, DateTime utcNow)
        {
            Name = name.Trim();
            UpdatedAt = utcNow;
        }
    }
}
