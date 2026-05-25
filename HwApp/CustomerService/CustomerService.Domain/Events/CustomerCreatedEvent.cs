using CustomerService.Domain.Interfaces;

namespace CustomerService.Domain.Events
{
    public record CustomerCreatedEvent(
            Guid UserId,
            string Name,
            string Email
        ) : IDomainEvent
    {
        public string Key => UserId.ToString();
    }
}
