namespace CatalogService.Infrastructure.Messaging.Kafka
{
    internal sealed class KafkaOptions
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; init; } = null!;

        public string GroupId { get; init; } = "catalog-service";

        public string[] Topics { get; init; } = [];

        public string CatalogProductTopic { get; init; } = "catalog.product";

        public string Acks { get; init; } = "All";

        public bool EnableIdempotence { get; init; } = true;
    }
}
