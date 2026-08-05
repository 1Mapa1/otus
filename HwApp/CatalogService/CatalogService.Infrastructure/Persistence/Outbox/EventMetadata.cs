namespace CatalogService.Infrastructure.Persistence.Outbox
{
    internal record EventMetadata(
        string Topic,
        string EventType,
        string Key);
}
