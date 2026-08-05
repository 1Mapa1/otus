using WarehouseService.Domain.Events;

namespace WarehouseService.Infrastructure.Persistence.Outbox
{
    internal interface IIntegrationEventMapping
    {
        EventMetadata Resolve(IDomainEvent domainEvent);
    }
}
