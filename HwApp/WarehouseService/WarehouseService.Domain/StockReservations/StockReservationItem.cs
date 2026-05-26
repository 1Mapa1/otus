namespace WarehouseService.Domain.StockReservations
{
    public sealed class StockReservationItem
    {
        public Guid Id { get; private set; }

        public Guid ReservationId { get; private set; }

        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        private StockReservationItem() { }

        private StockReservationItem(
            Guid reservationId,
            Guid productId,
            int quantity)
        {
            Id = Guid.NewGuid();
            ReservationId = reservationId;
            ProductId = productId;
            Quantity = quantity;
        }

        public static StockReservationItem Create(
            Guid reservationId,
            Guid productId,
            int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
            }

            return new StockReservationItem(reservationId, productId, quantity);
        }
    }
}
