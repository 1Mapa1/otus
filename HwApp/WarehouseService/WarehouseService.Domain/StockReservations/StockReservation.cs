namespace WarehouseService.Domain.StockReservations
{
    public sealed class StockReservation
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }

        public StockReservationStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? CanceledAt { get; private set; }

        private StockReservation() { }

        private StockReservation(
            Guid orderId,
            Guid userId,
            StockReservationStatus status)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            UserId = userId;
            Status = status;
            CreatedAt = DateTime.UtcNow;
        }

        public static StockReservation Create(
            Guid orderId,
            Guid userId)
        {
            return new StockReservation(
                orderId, 
                userId, 
                StockReservationStatus.Reserved);
        }

        public void Cancel()
        {
            if (Status == StockReservationStatus.Canceled)
                return;

            Status = StockReservationStatus.Canceled;
            CanceledAt = DateTime.UtcNow;
        }
    }
}
