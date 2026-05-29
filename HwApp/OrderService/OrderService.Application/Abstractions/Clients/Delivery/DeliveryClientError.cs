namespace OrderService.Application.Abstractions.Clients.Delivery
{
    public sealed record DeliveryClientError(
        DeliveryClientErrorCode Code,
        string? Message = null);
}
