using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities
{
    public sealed class User
    {
        public Guid Id { get; private set; }

        public string Login { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public UserStatus Status { get; private set; }

        public UserRole Role { get; private set; }

        public DateTime CreateAt { get; private set; }

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

        public void Activate() => Status = UserStatus.Active;

        public void Block() => Status = UserStatus.Blocked;
    }
}
