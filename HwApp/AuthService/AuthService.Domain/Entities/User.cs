using AuthService.Domain.Enums;
using AuthService.Domain.Events;
using AuthService.Domain.Interfaces;

namespace AuthService.Domain.Entities
{
    public sealed class User : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _events = [];

        public Guid Id { get; private set; }

        public string Login { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public UserStatus Status { get; private set; }

        public UserRole Role { get; private set; }

        public DateTime CreateAt { get; private set; }

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        private User() { }

        public User(string login, string passwordHash, UserRole role = UserRole.User)
        {
            Id = Guid.NewGuid();
            Login = login;
            PasswordHash = passwordHash;
            Status = UserStatus.Pending;
            CreateAt = DateTime.UtcNow;
            Role = role;
        }

        public void Activate()
        {
            Status = UserStatus.Active;
            AddEvent(new UserActivatedEvent(Id));
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash must not be empty.", nameof(passwordHash));

            PasswordHash = passwordHash;
        }

        public void Block() => Status = UserStatus.Blocked;

        public void AddEvent(IDomainEvent domainEvent)
            => _events.Add(domainEvent);

        public void ClearEvents()
            => _events.Clear();
    }
}
