using CatalogService.Domain.Events;

namespace CatalogService.Infrastructure.Persistence.Outbox
{
    internal interface IIntegrationEventMapping
    {
        EventMetadata Resolve(IDomainEvent domainEvent);
    }
}
