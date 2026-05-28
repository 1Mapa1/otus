namespace OrderService.Infrastructure.Clients.Billing.Responses
{
    internal sealed record AuthorizePaymentResponse(
        Guid PaymentId,
        decimal AuthorizedAmount);
}
