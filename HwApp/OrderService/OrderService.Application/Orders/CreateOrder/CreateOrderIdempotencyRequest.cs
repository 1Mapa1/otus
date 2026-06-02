namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderIdempotencyRequest(
        Guid DeliverySlotId,
        IReadOnlyCollection<CreateOrderItem> Items);
}
