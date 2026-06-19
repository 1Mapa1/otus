using MediatR;
using OrderService.Application.Abstractions.Clients.Warehouse;
using OrderService.Application.Abstractions.Clients.Warehouse.ResolveProducts;
using OrderService.Application.Abstractions.Persistence;
using OrderService.Application.Idempotency;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    internal sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseClient _warehouseClient;
        private readonly IIdempotencyService _idempotencyService;

        public CreateOrderHandler(
            IOrderRepository orderRepository,
            IWarehouseClient warehouseClient,
            IIdempotencyService idempotencyService,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _warehouseClient = warehouseClient;
            _idempotencyService = idempotencyService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var idempotency = await _idempotencyService.StartAsync<CreateOrderIdempotencyRequest, CreateOrderResult>(
                request.UserId,
                request.IdempotencyKey,
                new CreateOrderIdempotencyRequest(request.DeliverySlotId, request.Items),
                cancellationToken);

            if (idempotency.IsConflict)
                return CreateOrderResult.IdempotencyKeyConflict();

            if (idempotency.IsAlreadyProcessing)
                return CreateOrderResult.RequestAlreadyProcessing();

            if (idempotency.IsCompleted)
                return idempotency.SavedCreateOrderResult!;

            var result = await _warehouseClient.ResolveProductsAsync(request.Items.Select(x =>  new ResolveProductItem(x.ProductId, x.Quantity)).ToArray(), cancellationToken);

            if(!result.IsSuccess)
                return CreateOrderResult.WarehouseResolveFailed(result.Error?.Message);

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

            var orderResult = CreateOrderResult.Success(order.Id, order.Status);

            _idempotencyService.Complete(idempotency.Record!, order.Id, orderResult);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return orderResult;
        }
    }
}
