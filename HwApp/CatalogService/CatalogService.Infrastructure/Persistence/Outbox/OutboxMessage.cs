namespace CatalogService.Infrastructure.Persistence.Outbox
{
    internal sealed class OutboxMessage
    {
        public Guid Id { get; set; }

        public string Topic { get; set; } = null!;

        public string Key { get; set; } = null!;

        public string Payload { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }
    }
}
