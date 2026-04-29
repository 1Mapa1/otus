using CustomerService.Domain.Interfaces;

namespace CustomerService.Infrastructure.Persistence.Outbox
{
    internal interface IIntegrationEventMapping
    {
        EventMetadata Resolve(IDomainEvent domainEvent);
    }
}
