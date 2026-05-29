namespace DeliveryService.Domain.Slots
{
    public sealed class DeliverySlot
    {
        public Guid Id { get; private set; }

        public DateTime TimeFrom { get; private set; }

        public DateTime TimeTo { get; private set; }

        public DeliverySlotStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private DeliverySlot()
        {
        }

        public static DeliverySlot Create(DateTime timeFrom, DateTime timeTo)
        {
            var now = DateTime.UtcNow;

            return new DeliverySlot
            {
                Id = Guid.NewGuid(),
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                Status = DeliverySlotStatus.Available,
                CreatedAt = now,
                UpdatedAt = now
            };
        }
    }
}
