using NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers;

namespace NotificationService.Infrastructure.Messaging.Kafka
{
    internal sealed class KafkaMessageDispatcher
    {
        private readonly IReadOnlyDictionary<string, IKafkaIntegrationEventHandler> _handlers;

        public KafkaMessageDispatcher(
            IEnumerable<IKafkaIntegrationEventHandler> handlers)
        {
            _handlers = handlers.ToDictionary(
                x => x.EventType,
                StringComparer.OrdinalIgnoreCase);
        }

        public bool CanHandle(string eventType)
        {
            return _handlers.ContainsKey(eventType);
        }

        public async Task DispatchAsync(
            KafkaIntegrationEventEnvelope envelope,
            CancellationToken cancellationToken)
        {
            if (!_handlers.TryGetValue(envelope.EventType, out var handler))
            {
                throw new DeadLetterMessageException(
                    $"Unsupported Kafka event type: {envelope.EventType}");
            }

            await handler.HandleAsync(envelope.Data, cancellationToken);
        }
    }
}
