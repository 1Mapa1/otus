namespace OrderService.Infrastructure.Persistence.Outbox
{
    internal sealed class OutboxMessage
    {
        public Guid Id { get; set; }

        public string Topic { get; set; } = default!;

        public string Key { get; set; } = default!;

        public string Payload { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }
    }
}
