namespace WarehouseService.Domain.Stocks
{
    public sealed class StockMovement
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }

        public StockMovementType Type { get; private set; }

        public int Quantity { get; private set; }

        public Guid? OrderId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private StockMovement()
        {
        }

        private StockMovement(
            Guid productId,
            StockMovementType type,
            int quantity,
            Guid? orderId,
            DateTime createdAt)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Type = type;
            Quantity = quantity;
            OrderId = orderId;
            CreatedAt = createdAt;
        }

        public static StockMovement CreateIncome(Guid productId, int quantity, DateTime createdAt)
        {
            EnsurePositiveQuantity(quantity);

            return new StockMovement(
                productId,
                StockMovementType.Income,
                quantity,
                orderId: null,
                createdAt);
        }

        public static StockMovement CreateReservationCreated(
            Guid productId,
            int quantity,
            Guid orderId,
            DateTime createdAt)
        {
            EnsurePositiveQuantity(quantity);

            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId is required.", nameof(orderId));

            return new StockMovement(
                productId,
                StockMovementType.ReservationCreated,
                quantity,
                orderId,
                createdAt);
        }

        public static StockMovement CreateReservationCanceled(
            Guid productId,
            int quantity,
            Guid orderId,
            DateTime createdAt)
        {
            EnsurePositiveQuantity(quantity);

            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId is required.", nameof(orderId));

            return new StockMovement(
                productId,
                StockMovementType.ReservationCanceled,
                quantity,
                orderId,
                createdAt);
        }

        private static void EnsurePositiveQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }
    }
}
