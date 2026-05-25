namespace BillingService.Domain.AccountTransactions
{
    public sealed class AccountTransaction
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid? OrderId { get; private set; }

        public Guid? PaymentId { get; private set; }

        public AccountTransactionType Type { get; private set; }

        public decimal Amount { get; private set; }

        public decimal BalanceBefore { get; private set; }

        public decimal HeldBefore { get; private set; }

        public decimal BalanceAfter { get; private set; }

        public decimal HeldAfter { get; private set; }

        public DateTime CreatedAt{ get; private set; }

        private AccountTransaction() { }

        public static AccountTransaction Deposit(
            Guid userId,
            decimal amount,
            AccountBalanceSnapshot before,
            AccountBalanceSnapshot after)
        {
            return Create(
                userId,
                null,
                null,
                AccountTransactionType.Deposit,
                amount,
                before.Balance,
                before.HeldAmount,
                after.Balance,
                after.HeldAmount);
        }
        
        public static AccountTransaction Authorize(
            Guid userId,
            Guid orderId,
            Guid paymentId,
            decimal amount,
            AccountBalanceSnapshot before,
            AccountBalanceSnapshot after)
        {
            return Create(
                userId,
                orderId,
                paymentId,
                AccountTransactionType.Authorize,
                amount,
                before.Balance,
                before.HeldAmount,
                after.Balance,
                after.HeldAmount);
        }

        public static AccountTransaction Capture(
            Guid userId,
            Guid orderId,
            Guid paymentId,
            decimal amount,
            AccountBalanceSnapshot before,
            AccountBalanceSnapshot after)
        {
            return Create(
                userId,
                orderId,
                paymentId,
                AccountTransactionType.Capture,
                amount,
                before.Balance,
                before.HeldAmount,
                after.Balance,
                after.HeldAmount);
        }

        public static AccountTransaction CancelAuthorization(
            Guid userId,
            Guid orderId,
            Guid paymentId,
            decimal amount,
            AccountBalanceSnapshot before,
            AccountBalanceSnapshot after)
        {
            return Create(
                userId,
                orderId,
                paymentId,
                AccountTransactionType.CancelAuthorization,
                amount,
                before.Balance,
                before.HeldAmount,
                after.Balance,
                after.HeldAmount);
        }

        private static AccountTransaction Create(
            Guid userId,
            Guid? orderId,
            Guid? paymentId,
            AccountTransactionType type,
            decimal amount,
            decimal balanceBefore,
            decimal heldBefore,
            decimal balanceAfter,
            decimal heldAfter)
        {
            return new AccountTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderId = orderId,
                PaymentId = paymentId,
                Type = type,
                Amount = amount,
                BalanceBefore = balanceBefore,
                HeldBefore = heldBefore,
                BalanceAfter = balanceAfter,
                HeldAfter = heldAfter,
                CreatedAt= DateTime.UtcNow
            };
        }
    }
}