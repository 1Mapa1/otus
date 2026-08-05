namespace WarehouseService.Infrastructure.Messaging.Kafka
{
    internal sealed class KafkaOptions
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; init; } = null!;

        public string GroupId { get; init; } = "warehouse-service";

        public string[] Topics { get; init; } = [];

        public string WarehouseStockTopic { get; init; } = null!;

        public string Acks { get; init; } = "All";

        public bool EnableIdempotence { get; init; } = true;
    }
}
