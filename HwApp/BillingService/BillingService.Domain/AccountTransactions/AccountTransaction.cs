namespace BillingService.Domain.AccountTransactions
{
    public sealed class AccountTransaction
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid? OrderId { get; private set; }

        public AccountTransactionType Type { get; private set; }

        public decimal Amount { get; private set; }

        public decimal BalanceBefore { get; private set; }

        public decimal BalanceAfter { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public static AccountTransaction Create(
            Guid userId,
            Guid? orderId,
            AccountTransactionType type,
            decimal amount,
            decimal balanceBefore,
            decimal balanceAfter)
        {
            return new AccountTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderId = orderId,
                Type = type,
                Amount = amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}