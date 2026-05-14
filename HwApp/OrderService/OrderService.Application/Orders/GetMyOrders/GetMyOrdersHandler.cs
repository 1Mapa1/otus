using MediatR;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.GetMyOrders
{
    internal sealed class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IOrderRepository _orders;

        public GetMyOrdersHandler(IOrderRepository orders)
        {
            _orders = orders;
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(
            GetMyOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _orders.GetByUserIdAsync(request.UserId, cancellationToken);

            return items
                .Select(o => new OrderDto(o.Id, o.Status.ToString(), o.CreatedAt))
                .ToList();
        }
    }
}
