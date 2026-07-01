using MediatR;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderCommand(
        Guid UserId,
        Guid DeliverySlotId,
        DeliveryAddressSnapshot DeliveryAddress,
        IReadOnlyList<CreateOrderItem> Items,
        Guid IdempotencyKey) : IRequest<CreateOrderResult>;
}
