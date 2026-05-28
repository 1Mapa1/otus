namespace OrderService.Application.Abstractions.Delivery
{
    public sealed record DeliveryClientError(
        DeliveryClientErrorCode Code,
        string? Message = null);
}
