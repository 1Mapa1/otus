namespace NotificationService.Domain.Customers
{
    public sealed class NotificationCustomer
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public DateTime UpdatedAt { get; private set; }

        private NotificationCustomer() { }

        public static NotificationCustomer Create(Guid id, string name, string email)
        {
            return new NotificationCustomer
            {
                Id = id,
                Name = name,
                Email = email,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void Update(string name, string email)
        {
            Name = name;
            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
