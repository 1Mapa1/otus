using System.Text.Json;

namespace WarehouseService.Infrastructure.Messaging.Contracts
{
    internal sealed record IntegrationEventEnvelope(
        Guid EventId,
        string EventType,
        DateTime OccurredAt,
        JsonElement Data);
}
