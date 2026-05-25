namespace OrderService.Infrastructure.Clients.BillingService.Requests
{
    internal sealed record WithdrawRequest(
        Guid UserId,
        Guid OrderId,
        decimal Amount);
}
