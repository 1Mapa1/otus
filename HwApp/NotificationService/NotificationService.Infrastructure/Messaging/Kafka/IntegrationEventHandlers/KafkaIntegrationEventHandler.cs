using MediatR;
using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging.Kafka.IntegrationEventHandlers
{
    internal abstract class KafkaIntegrationEventHandler<TEvent> : IKafkaIntegrationEventHandler
         where TEvent : class
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        protected readonly ISender _sender;

        protected KafkaIntegrationEventHandler(ISender sender)
        {
            _sender = sender;
        }

        public abstract string EventType { get; }

        public async Task HandleAsync(
         JsonElement data,
         CancellationToken cancellationToken)
        {
            var integrationEvent = data.Deserialize<TEvent>(JsonOptions);

            if (integrationEvent is null)
            {
                throw new InvalidOperationException(
                    $"Cannot deserialize Kafka event data to {typeof(TEvent).Name}.");
            }

            await HandleAsync(integrationEvent, cancellationToken);
        }

        protected abstract Task HandleAsync(
            TEvent integrationEvent,
            CancellationToken cancellationToken);
    }
}
