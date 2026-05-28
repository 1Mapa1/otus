using OrderService.Domain.Events;
using OrderService.Domain.Orders.Events;

namespace OrderService.Infrastructure.Persistence.Outbox
{
    internal sealed class IntegrationEventMapping : IIntegrationEventMapping
    {
        private static readonly IReadOnlyDictionary<Type, (string Topic, string EventType)> Mapping =
           new Dictionary<Type, (string Topic, string EventType)>
           {
               [typeof(OrderConfirmedEvent)] = ("orders", "order.confirmed.v1"),
               [typeof(OrderRejectedEvent)] = ("orders", "order.rejected.v2")
           };

        public EventMetadata Resolve(IDomainEvent domainEvent)
        {
            if (!Mapping.TryGetValue(domainEvent.GetType(), out var metadata))
                throw new InvalidOperationException(
                    $"No integration mapping for {domainEvent.GetType().Name}");

            return new EventMetadata(
                metadata.Topic,
                metadata.EventType,
                domainEvent.Key);
        }
    }
}
