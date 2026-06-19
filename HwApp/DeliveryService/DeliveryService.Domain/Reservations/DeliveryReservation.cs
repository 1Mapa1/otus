namespace DeliveryService.Domain.Reservations
{
    public sealed class DeliveryReservation
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid UserId { get; private set; }

        public Guid DeliverySlotId { get; private set; }

        public DeliveryReservationStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? CanceledAt { get; private set; } = null;

        private DeliveryReservation()
        {
        }

        public static DeliveryReservation Create(Guid orderId, Guid userId, Guid deliverySlotId)
        {
            var now = DateTime.UtcNow;
            return new DeliveryReservation
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                UserId = userId,
                DeliverySlotId = deliverySlotId,
                Status = DeliveryReservationStatus.Reserved,
                CreatedAt = now
            };
        }
    }
}
