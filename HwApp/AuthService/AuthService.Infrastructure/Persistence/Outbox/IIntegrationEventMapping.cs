using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Persistence.Outbox
{
    internal interface IIntegrationEventMapping
    {
        EventMetadata Resolve(IDomainEvent domainEvent);
    }
}
