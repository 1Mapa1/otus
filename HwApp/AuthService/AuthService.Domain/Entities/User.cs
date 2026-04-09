using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities
{
    public sealed class User
    {
        public Guid Id { get; private set; }

        public string Login { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public UserStatus Status { get; private set; }

        public DateTime CreateAt { get; private set; }

        private User() { }

        public User(string login, string passwordHash)
        {
            Id = new Guid();
            Login = login;
            PasswordHash = passwordHash;
            Status = UserStatus.Pending;
            CreateAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            Status = UserStatus.Active;
        }

        public void Blocked()
        {
            Status = UserStatus.Blocked;
        }
    }
}
