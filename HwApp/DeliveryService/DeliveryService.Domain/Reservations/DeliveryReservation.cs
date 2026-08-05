namespace DeliveryService.Domain.Reservations
{
    public sealed class DeliveryReservation
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid CustomerId { get; private set; }

        public Guid DeliverySlotId { get; private set; }

        public Guid ZoneId { get; private set; }

        public DeliveryReservationStatus Status { get; private set; }

        public DeliveryAddressSnapshot DeliveryAddress { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public DateTime? CanceledAt { get; private set; }

        private DeliveryReservation()
        {
        }

        public static DeliveryReservation Create(
            Guid orderId,
            Guid customerId,
            Guid deliverySlotId,
            Guid zoneId,
            DeliveryAddressSnapshot address)
        {
            var now = DateTime.UtcNow;

            return new DeliveryReservation
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                CustomerId = customerId,
                DeliverySlotId = deliverySlotId,
                ZoneId = zoneId,
                Status = DeliveryReservationStatus.Reserved,
                DeliveryAddress = address,
                CreatedAt = now
            };
        }
    }
}
