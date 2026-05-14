using MediatR;

namespace OrderService.Application.Orders.GetMyOrders
{
    public sealed record GetMyOrdersQuery(
        Guid UserId) : IRequest<IReadOnlyList<OrderDto>>;
}
