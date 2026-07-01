namespace CatalogService.Domain.Brands
{
    public sealed class Brand
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private Brand()
        {
        }

        public static Brand Create(string name, DateTime utcNow)
        {
            return new Brand
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
