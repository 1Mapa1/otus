namespace WarehouseService.Domain.Stocks
{
    public sealed class StockItem
    {
        public Guid ProductId { get; private set; }

        public int AvailableQuantity { get; private set; }

        public int ReservedQuantity { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private StockItem()
        {
        }

        private StockItem(Guid productId)
        {
            ProductId = productId;
            AvailableQuantity = 0;
            ReservedQuantity = 0;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public static StockItem Create(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId is required.", nameof(productId));

            return new StockItem(productId);
        }

        public void IncreaseAvailableQuantity(int quantity)
        {
            EnsurePositiveQuantity(quantity);

            AvailableQuantity += quantity;
            Touch();
        }

        public void Reserve(int quantity)
        {
            EnsurePositiveQuantity(quantity);

            if (!IsActive)
                throw new InvalidOperationException("Cannot reserve stock for an inactive item.");

            if (AvailableQuantity < quantity)
                throw new InvalidOperationException("Insufficient available quantity.");

            AvailableQuantity -= quantity;
            ReservedQuantity += quantity;
            Touch();
        }

        public void CancelReservation(int quantity)
        {
            EnsurePositiveQuantity(quantity);

            if (ReservedQuantity < quantity)
                throw new InvalidOperationException("Insufficient reserved quantity.");

            ReservedQuantity -= quantity;
            AvailableQuantity += quantity;
            Touch();
        }

        public void Archive()
        {
            IsActive = false;
            Touch();
        }

        public void Restore()
        {
            IsActive = true;
            Touch();
        }

        private void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        private static void EnsurePositiveQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }
    }
}
