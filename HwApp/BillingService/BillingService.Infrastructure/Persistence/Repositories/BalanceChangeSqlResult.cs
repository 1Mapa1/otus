namespace BillingService.Infrastructure.Persistence.Repositories
{
    internal sealed class BalanceChangeSqlResult
    {
        public decimal BalanceBefore { get; init; }

        public decimal BalanceAfter { get; init; }

        public decimal HeldAmountAfter { get; init; }

        public decimal HeldAmountBefore { get; init; }
    }
}
