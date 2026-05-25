using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal interface IKafkaIntegrationEventHandler
    {
        string EventType { get; }

        Task HandleAsync(
            JsonElement data,
            CancellationToken cancellationToken);
    }
}
