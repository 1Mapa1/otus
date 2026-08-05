namespace NotificationService.Infrastructure.Persistence.Entities
{
    internal sealed class InboxMessage
    {
        public Guid EventId { get; private set; }
        public string EventType { get; private set; } = null!;

        public string KafkaTopic { get; private set; } = null!;
        public int KafkaPartition { get; private set; }
        public long KafkaOffset { get; private set; }

        public DateTime OccurredAtUtc { get; private set; }
        public DateTime ProcessedAtUtc { get; private set; }

        private InboxMessage() { }

        private InboxMessage(
            Guid eventId,
            string eventType,
            DateTime occurredAtUtc,
            string kafkaTopic,
            int kafkaPartition,
            long kafkaOffset)
        {
            EventId = eventId;
            EventType = eventType;
            OccurredAtUtc = occurredAtUtc;
            ProcessedAtUtc = DateTime.UtcNow;
            KafkaTopic = kafkaTopic;
            KafkaPartition = kafkaPartition;
            KafkaOffset = kafkaOffset;
        }

        public static InboxMessage Create(
            Guid eventId,
            string eventType,
            DateTime occurredAtUtc,
            string kafkaTopic,
            int kafkaPartition,
            long kafkaOffset)
        {
            return new InboxMessage(
                eventId,
                eventType,
                occurredAtUtc,
                kafkaTopic,
                kafkaPartition,
                kafkaOffset);
        }
    }
}
