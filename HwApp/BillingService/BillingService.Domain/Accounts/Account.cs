namespace BillingService.Domain.Accounts
{
    public sealed class Account
    {
        public Guid UserId { get; private set; }

        public decimal Balance { get; private set; }

        public decimal HeldAmount { get; private set; }

        public decimal AvailableBalance => Balance - HeldAmount;

        public DateTime CreatedAt{ get; private set; }

        public DateTime UpdatedAt{ get; private set; }

        private Account() { }

        public static Account Create(Guid userId)
        {
            var utcNow = DateTime.UtcNow;

            return new Account
            {
                UserId = userId,
                Balance = 0,
                HeldAmount = 0,
                CreatedAt= utcNow,
                UpdatedAt= utcNow
            };
        }
    }
}
