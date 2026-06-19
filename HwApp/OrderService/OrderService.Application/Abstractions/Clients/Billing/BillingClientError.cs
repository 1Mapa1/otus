namespace OrderService.Application.Abstractions.Clients.Billing
{
    public sealed record BillingClientError(
        BillingClientErrorCode Code,
        string? Message = null);
}
