using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderResult(
        CreateOrderResultStatus ResultStatus,
        Guid? OrderId,
        OrderStatus? OrderStatus,
        string? FailureReason,
        IReadOnlyList<CreateOrderPriceChangedItem>? PriceChangedItems = null)
    {
        public static CreateOrderResult Success(Guid orderId, OrderStatus orderStatus) =>
            new(CreateOrderResultStatus.Success, orderId, orderStatus, null);

        public static CreateOrderResult IdempotencyKeyConflict() =>
            new(CreateOrderResultStatus.IdempotencyKeyConflict, null, null, null);

        public static CreateOrderResult RequestAlreadyProcessing() =>
            new(CreateOrderResultStatus.RequestAlreadyProcessing, null, null, null);

        public static CreateOrderResult CatalogSnapshotFailed(string? reason) =>
            new(CreateOrderResultStatus.CatalogSnapshotFailed, null, null, reason);

        public static CreateOrderResult PriceChanged(IReadOnlyList<CreateOrderPriceChangedItem> items) =>
            new(CreateOrderResultStatus.PriceChanged, null, null, null, items);
    }
}
