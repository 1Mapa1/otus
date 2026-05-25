using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging.Kafka
{
    public sealed record KafkaIntegrationEventEnvelope(
        Guid EventId,
        string EventType,
        DateTime OccurredAt,
        JsonElement Data);
}
