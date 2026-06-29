using System.Text.Json;

namespace BillingService.Infrastructure.Messaging.Contracts
{
    internal sealed record IntegrationEventEnvelope(
        Guid EventId,
        string EventType,
        DateTime OccurredAt,
        JsonElement Data);
}
