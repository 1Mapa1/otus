using OrderService.Application.Orders.CreateOrder;

namespace OrderService.Api.Contracts
{ 
    public sealed record CreateOrderRequest(
        Guid DeliverySlotId,
        IReadOnlyList<CreateOrderItem> Items);
}
