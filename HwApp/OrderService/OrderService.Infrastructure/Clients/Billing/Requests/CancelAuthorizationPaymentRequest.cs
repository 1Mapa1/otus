namespace OrderService.Infrastructure.Clients.Billing.Requests
{
    internal sealed record CancelAuthorizationPaymentRequest(Guid OrderId);
}
