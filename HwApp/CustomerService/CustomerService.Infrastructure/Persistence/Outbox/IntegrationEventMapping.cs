using CustomerService.Domain.Events;
using CustomerService.Domain.Interfaces;

namespace CustomerService.Infrastructure.Persistence.Outbox
{
    internal sealed class IntegrationEventMapping : IIntegrationEventMapping
    {
        private static readonly IReadOnlyDictionary<Type, EventMetadata> mapping = new Dictionary<Type, EventMetadata>
        {
            [typeof(CustomerCreatedEvent)] = new EventMetadata("customers", "customer.created.v1"),
            [typeof(CustomerUpdatedEvent)] = new EventMetadata("customers", "customer.updated.v1")
        };

        public EventMetadata Resolve(IDomainEvent domainEvent)
        {
            if (!mapping.TryGetValue(domainEvent.GetType(), out var metadata))
                throw new InvalidOperationException($"No integration mapping for {domainEvent.GetType().Name}");

            return metadata;
        }
    }
}
