using MediatR;

namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderCommand(
        Guid UserId,
        decimal Price) : IRequest<CreateOrderResult>;
}
