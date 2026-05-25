namespace BillingService.Api.Contracts
{
    public sealed record GetMyAccountResponse(
        Guid UserId,
        decimal Balance);
}
