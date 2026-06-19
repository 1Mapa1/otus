namespace OrderService.Infrastructure.Clients.Billing.Requests
{
    internal sealed record AuthorizePaymentRequest(
        Guid UserId,
        Guid OrderId,
        decimal Amount);
}
