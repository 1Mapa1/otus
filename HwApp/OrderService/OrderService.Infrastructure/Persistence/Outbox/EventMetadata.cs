namespace OrderService.Infrastructure.Persistence.Outbox
{
    internal record EventMetadata(
            string Topic,
            string EventType,
            string Key
        );
}
