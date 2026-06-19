namespace BillingService.Api.Contracts
{
    public sealed record CapturePaymentRequest(
        Guid OrderId);
}
