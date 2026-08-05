using DeliveryService.Domain.Zones;

namespace DeliveryService.Domain.Slots
{
    public sealed class DeliverySlot
    {
        public Guid Id { get; private set; }

        public Guid ZoneId { get; private set; }

        public DeliveryZone Zone { get; private set; } = null!;

        public DateTime TimeFrom { get; private set; }

        public DateTime TimeTo { get; private set; }

        public int Capacity { get; private set; }

        public int ReservedCount { get; private set; }

        public DeliverySlotStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private DeliverySlot()
        {
        }

        public static DeliverySlot Create(
            Guid zoneId,
            DateTime timeFrom,
            DateTime timeTo,
            int capacity)
        {
            var now = DateTime.UtcNow;

            return new DeliverySlot
            {
                Id = Guid.NewGuid(),
                ZoneId = zoneId,
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                Capacity = capacity,
                ReservedCount = 0,
                Status = DeliverySlotStatus.Draft,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public void UpdateDraft(
            Guid zoneId,
            DateTime timeFrom,
            DateTime timeTo,
            int capacity,
            DeliverySlotStatus status)
        {
            ZoneId = zoneId;
            TimeFrom = timeFrom;
            TimeTo = timeTo;
            Capacity = capacity;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOpen(int capacity, DeliverySlotStatus status)
        {
            Capacity = capacity;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateClosed(DeliverySlotStatus status, int capacity)
        {
            Status = status;
            Capacity = capacity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
