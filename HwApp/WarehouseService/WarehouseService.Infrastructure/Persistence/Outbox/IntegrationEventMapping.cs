using WarehouseService.Domain.Events;
using WarehouseService.Domain.Stocks.Events;

namespace WarehouseService.Infrastructure.Persistence.Outbox
{
    internal sealed class IntegrationEventMapping : IIntegrationEventMapping
    {
        private static readonly IReadOnlyDictionary<Type, (string Topic, string EventType)> Mapping =
            new Dictionary<Type, (string Topic, string EventType)>
            {
                [typeof(StockChangedEvent)] = ("warehouse.stock", "StockChanged")
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
