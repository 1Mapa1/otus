using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Abstractions.Clients.Warehouse;
using OrderService.Application.Abstractions.Clients.Warehouse.ResolveProducts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    internal sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseClient _warehouseClient;

        public CreateOrderHandler(IOrderRepository orderRepository, IWarehouseClient warehouseClient, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _warehouseClient = warehouseClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var result = await _warehouseClient.ResolveProductsAsync(request.Items.Select(x =>  new ResolveProductItem(x.ProductId, x.Quantity)).ToArray(), cancellationToken);

            if(!result.IsSuccess)
                return new CreateOrderResult(Guid.Empty, OrderStatus.Rejected, result.Error?.Message);

            var order = Order.Create(request.UserId, request.DeliverySlotId, result.TotalAmount);

            foreach (var item in result.Items)
            {
                order.AddItem(
                    item.ProductId,
                    item.Name,
                    item.UnitPrice,
                    item.Quantity,
                    item.TotalPrice);
            }

            await _orderRepository.AddAsync(order, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateOrderResult(order.Id, order.Status, null);
        }
    }
}
