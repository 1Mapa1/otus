namespace OrderService.Infrastructure.Clients.Billing.Responses
{
    internal sealed record CapturePaymentResponse(
        Guid PaymentId,
        decimal AuthorizedAmount);
}
