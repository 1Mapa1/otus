namespace BillingService.Application.Accounts.GetMyAccount
{
    public sealed record AccountDto(
        Guid UserId,
        decimal Balance,
        decimal HeldAmount,
        decimal AvailableBalance);
}