using CustomerService.Domain.Interfaces;

namespace CustomerService.Domain.Events
{
    public record class CustomerUpdatedEvent(
            Guid UserId,
            string Name,
            string Email
        ) : IDomainEvent
    {
        public string Key => UserId.ToString();
    }
}
