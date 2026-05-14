using OrderService.Domain.Events;

namespace OrderService.Infrastructure.Persistence.Outbox
{
    internal interface IIntegrationEventMapping
    {
        EventMetadata Resolve(IDomainEvent domainEvent);
    }
}
