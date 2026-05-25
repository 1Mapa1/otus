using MediatR;

namespace OrderService.Application.Orders.GetOrderById
{
    public sealed record GetOrderByIdQuery(
        Guid UserId,
        Guid OrderId) : IRequest<OrderDetailsDto?>;
}
