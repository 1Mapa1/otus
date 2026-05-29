namespace BillingService.Api.Contracts
{
    public sealed record AuthorizePaymentRequest(
        Guid UserId,
        Guid OrderId,
        decimal Amount);
}
