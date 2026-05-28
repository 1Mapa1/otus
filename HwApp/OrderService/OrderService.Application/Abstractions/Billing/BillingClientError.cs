namespace OrderService.Application.Abstractions.Billing
{
    public sealed record BillingClientError(
        BillingClientErrorCode Code,
        string? Message = null);
}
