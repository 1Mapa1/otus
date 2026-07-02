using CatalogService.Domain.Events;
using CatalogService.Domain.Products.Events;

namespace CatalogService.Infrastructure.Persistence.Outbox
{
    internal sealed class IntegrationEventMapping : IIntegrationEventMapping
    {
        private static readonly IReadOnlyDictionary<Type, (string Topic, string EventType)> Mapping =
            new Dictionary<Type, (string Topic, string EventType)>
            {
                [typeof(ProductCreatedEvent)] = ("products", "product.created.v1"),
                [typeof(ProductArchivedEvent)] = ("products", "product.archived.v1"),
                [typeof(ProductRestoredEvent)] = ("products", "product.restored.v1")
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
