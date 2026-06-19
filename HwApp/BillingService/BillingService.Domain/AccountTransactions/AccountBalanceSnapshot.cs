namespace BillingService.Domain.AccountTransactions
{
    public readonly record struct AccountBalanceSnapshot(
        decimal Balance,
        decimal HeldAmount);
}
