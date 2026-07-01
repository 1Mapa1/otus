using OrderService.Application.Orders.CreateOrder;

namespace OrderService.Api.Contracts
{
    public sealed record CreateOrderRequest(
        Guid DeliverySlotId,
        DeliveryAddressRequest DeliveryAddress,
        IReadOnlyList<CreateOrderItem> Items);
}
