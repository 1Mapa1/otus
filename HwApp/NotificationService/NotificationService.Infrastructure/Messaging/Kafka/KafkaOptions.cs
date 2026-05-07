namespace NotificationService.Infrastructure.Messaging.Kafka
{
    public sealed class KafkaOptions
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; init; } = null!;
        public string GroupId { get; init; } = "notification-service";
        public string[] Topics { get; init; } = [];
    }
}
