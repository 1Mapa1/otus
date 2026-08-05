using MediatR;
using OrderService.Application.Abstractions.Clients.Catalog;
using OrderService.Application.Abstractions.Persistence;
using OrderService.Application.Idempotency;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    internal sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICatalogClient _catalogClient;
        private readonly IIdempotencyService _idempotencyService;

        public CreateOrderHandler(
            IOrderRepository orderRepository,
            ICatalogClient catalogClient,
            IIdempotencyService idempotencyService,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _catalogClient = catalogClient;
            _idempotencyService = idempotencyService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var idempotency = await _idempotencyService.StartAsync<CreateOrderIdempotencyRequest, CreateOrderResult>(
                request.UserId,
                request.IdempotencyKey,
                new CreateOrderIdempotencyRequest(
                    request.DeliverySlotId,
                    request.DeliveryAddress,
                    request.Items),
                cancellationToken);

            if (idempotency.IsConflict)
                return CreateOrderResult.IdempotencyKeyConflict();

            if (idempotency.IsAlreadyProcessing)
                return CreateOrderResult.RequestAlreadyProcessing();

            if (idempotency.IsCompleted)
                return idempotency.SavedCreateOrderResult!;

            var validationFailure = ValidateRequest(request);
            if (validationFailure is not null)
            {
                _idempotencyService.Complete(idempotency.Record!, null, validationFailure);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return validationFailure;
            }

            var snapshotResult = await _catalogClient.GetSnapshotAsync(
                request.Items
                    .Select(item => new GetProductSnapshotItem(
                        item.ProductId,
                        item.Quantity,
                        item.ExpectedUnitPrice))
                    .ToArray(),
                cancellationToken);

            if (!snapshotResult.IsSuccess)
            {
                var failureResult = MapSnapshotFailure(snapshotResult.Error);

                _idempotencyService.Complete(idempotency.Record!, null, failureResult);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return failureResult;
            }

            var order = Order.Create(
                request.UserId,
                request.DeliverySlotId,
                request.DeliveryAddress,
                snapshotResult.TotalAmount);

            foreach (var item in snapshotResult.Items)
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

        private static CreateOrderResult MapSnapshotFailure(CatalogClientError? error)
        {
            if (error?.Code == CatalogClientErrorCode.PriceChanged)
            {
                var items = error.PriceChangedItems?
                    .Select(item => new CreateOrderPriceChangedItem(
                        item.ProductId,
                        item.ExpectedUnitPrice,
                        item.ActualUnitPrice))
                    .ToList()
                    ?? [];

                return CreateOrderResult.PriceChanged(items);
            }

            return CreateOrderResult.CatalogSnapshotFailed(error?.Message);
        }

        private static CreateOrderResult? ValidateRequest(CreateOrderCommand request)
        {
            if (request.Items is null || request.Items.Count == 0)
                return CreateOrderResult.CatalogSnapshotFailed("The order must contain at least one item.");

            if (request.Items.Any(item => item.ProductId == Guid.Empty || item.Quantity <= 0))
                return CreateOrderResult.CatalogSnapshotFailed("Each order item must have a product and a positive quantity.");

            if (request.Items.Any(item => item.ExpectedUnitPrice <= 0))
                return CreateOrderResult.CatalogSnapshotFailed("Each order item must have a positive expected unit price.");

            if (string.IsNullOrWhiteSpace(request.DeliveryAddress.City))
                return CreateOrderResult.CatalogSnapshotFailed("City is required.");

            if (string.IsNullOrWhiteSpace(request.DeliveryAddress.Street))
                return CreateOrderResult.CatalogSnapshotFailed("Street is required.");

            if (string.IsNullOrWhiteSpace(request.DeliveryAddress.House))
                return CreateOrderResult.CatalogSnapshotFailed("House is required.");

            return null;
        }
    }
}
