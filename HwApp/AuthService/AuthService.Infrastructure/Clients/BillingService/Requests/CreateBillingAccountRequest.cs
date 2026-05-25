namespace AuthService.Infrastructure.Clients.BillingService.Requests
{
    internal sealed record CreateBillingAccountRequest(
        Guid UserId);
}
