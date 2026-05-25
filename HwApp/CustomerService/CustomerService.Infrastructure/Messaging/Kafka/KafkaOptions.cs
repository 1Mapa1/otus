namespace CustomerService.Infrastructure.Messaging.Kafka
{
    internal class KafkaOptions
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; set; } = default!;

        public string Acks { get; set; } = default!;

        public bool EnableIdempotence { get; set; } = true;
    }
}
