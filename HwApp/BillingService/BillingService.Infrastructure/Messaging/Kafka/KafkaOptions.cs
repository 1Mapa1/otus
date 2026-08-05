namespace BillingService.Infrastructure.Messaging.Kafka
{
    internal sealed class KafkaOptions
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; init; } = null!;

        public string GroupId { get; init; } = "billing-service";

        public string[] Topics { get; init; } = [];

        public string DlqTopic { get; init; } = "billing.dlq";

        public int MaxRetryAttempts { get; init; } = 5;

        public int RetryDelaySeconds { get; init; } = 5;
    }
}
