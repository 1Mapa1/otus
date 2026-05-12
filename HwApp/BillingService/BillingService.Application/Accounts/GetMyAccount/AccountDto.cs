namespace BillingService.Application.Accounts.GetMyAccount
{
    public sealed record AccountDto(
        Guid UserId,
        decimal Balance,
        DateTime CreatedAt);
}