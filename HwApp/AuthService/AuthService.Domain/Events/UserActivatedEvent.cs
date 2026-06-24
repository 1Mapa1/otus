using AuthService.Domain.Interfaces;

namespace AuthService.Domain.Events
{
    public sealed record UserActivatedEvent(Guid UserId) : IDomainEvent
    {
        public string Key => UserId.ToString();
    }
}
