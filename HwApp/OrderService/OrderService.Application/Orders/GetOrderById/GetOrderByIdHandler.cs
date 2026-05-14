using MediatR;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.GetOrderById
{
    internal sealed class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto?>
    {
        private readonly IOrderRepository _orders;

        public GetOrderByIdHandler(IOrderRepository orders)
        {
            _orders = orders;
        }

        public async Task<OrderDetailsDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order is null || order.UserId != request.UserId)
                return null;

            return new OrderDetailsDto(
                order.Id,
                order.Status,
                order.FailureReason,
                order.Price,
                order.CreatedAt);
        }
    }
}
