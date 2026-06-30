namespace DeliveryService.Domain.Zones
{
    public sealed class DeliveryZone
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public string City { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private DeliveryZone()
        {
        }

        public static DeliveryZone Create(string name, string city)
        {
            var now = DateTime.UtcNow;

            return new DeliveryZone
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                City = city.Trim(),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public void Update(string name, bool isActive)
        {
            Name = name.Trim();
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
