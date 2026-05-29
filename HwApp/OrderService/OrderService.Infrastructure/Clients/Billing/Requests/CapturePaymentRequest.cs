namespace OrderService.Infrastructure.Clients.Billing.Requests
{
    internal sealed record CapturePaymentRequest(Guid OrderId);
}
