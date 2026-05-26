namespace WarehouseService.Domain.Products
{
    public sealed class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal UnitPrice { get; private set; }

        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public int FreeQuantity => AvailableQuantity - ReservedQuantity;

        private Product() { }

        private Product(string name, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            Name = name;
            UnitPrice = unitPrice;
            AvailableQuantity = 0;
            ReservedQuantity = 0;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public static Product Create(string name, decimal unitPrice)
        {
            return new Product(name, unitPrice);
        }

        public void IncreaseAvailableQuantity(int quantity)
        {
            AvailableQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncreaseReservedQuantity(int quantity)
        {
            ReservedQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecreaseReservedQuantity(int quantity)
        {
            ReservedQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
