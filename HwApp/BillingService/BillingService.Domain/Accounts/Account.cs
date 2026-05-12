namespace BillingService.Domain.Accounts
{
    public sealed class Account
    {
        public Guid UserId { get; private set; }

        public decimal Balance { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Account Create(Guid userId)
        {
            var utcNow = DateTime.UtcNow;

            return new Account
            {
                UserId = userId,
                Balance = 0,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
        }
    }
}
