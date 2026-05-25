using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers;
using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging.Kafka
{
    internal sealed class KafkaMessageDispatcher
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IReadOnlyDictionary<string, IKafkaIntegrationEventHandler> _handlers;

        public KafkaMessageDispatcher(
            IEnumerable<IKafkaIntegrationEventHandler> handlers)
        {
            _handlers = handlers.ToDictionary(
                x => x.EventType,
                StringComparer.OrdinalIgnoreCase);
        }

        public async Task DispatchAsync(
            string message,
            CancellationToken cancellationToken)
        {
            var envelope = JsonSerializer.Deserialize<KafkaIntegrationEventEnvelope>(
                message,
                JsonOptions);

            if (envelope is null)
                throw new InvalidOperationException("Kafka message envelope is empty.");

            if (!_handlers.TryGetValue(envelope.EventType, out var handler))
                throw new InvalidOperationException(
                    $"Unsupported Kafka event type: {envelope.EventType}");

            await handler.HandleAsync(envelope.Data, cancellationToken);
        }
    }
}
