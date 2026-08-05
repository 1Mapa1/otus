using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderIdempotencyRequest(
        Guid DeliverySlotId,
        DeliveryAddressSnapshot DeliveryAddress,
        IReadOnlyCollection<CreateOrderItem> Items);
}
