namespace BillingService.Api.Contracts
{
    public sealed record CancelAuthorizationPaymentRequest(
        Guid OrderId);
}
