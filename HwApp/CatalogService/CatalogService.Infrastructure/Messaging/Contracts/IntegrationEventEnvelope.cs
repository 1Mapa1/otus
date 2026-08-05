using System.Text.Json;

namespace CatalogService.Infrastructure.Messaging.Contracts
{
    internal sealed record IntegrationEventEnvelope(
        Guid EventId,
        string EventType,
        DateTime OccurredAt,
        JsonElement Data);
}
