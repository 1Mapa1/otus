namespace BillingService.Api.Contracts
{
    public sealed record WithdrawRequest(
        Guid UserId,
        Guid OrderId,
        decimal Amount);
}
