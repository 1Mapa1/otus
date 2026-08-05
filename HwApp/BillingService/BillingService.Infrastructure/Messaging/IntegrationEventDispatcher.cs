using BillingService.Infrastructure.Messaging.Contracts;
using BillingService.Infrastructure.Messaging.Handlers;

namespace BillingService.Infrastructure.Messaging
{
    internal sealed class IntegrationEventDispatcher
    {
        private readonly IReadOnlyDictionary<string, IIntegrationEventHandler[]> _handlersByEventType;

        public IntegrationEventDispatcher(
            IEnumerable<IIntegrationEventHandler> handlers)
        {
            _handlersByEventType = handlers
                .GroupBy(x => x.EventType, StringComparer.Ordinal)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray(),
                    StringComparer.Ordinal);
        }

        public bool CanHandle(string eventType)
        {
            return _handlersByEventType.ContainsKey(eventType);
        }

        public async Task DispatchAsync(
            IntegrationEventEnvelope message,
            CancellationToken cancellationToken)
        {
            if (!_handlersByEventType.TryGetValue(
                    message.EventType,
                    out var handlers))
            {
                throw new DeadLetterMessageException(
                    $"No handler registered for '{message.EventType}'.");
            }

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(message, cancellationToken);
            }
        }
    }
}
