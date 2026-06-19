using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    public sealed record CreateOrderResult(
        CreateOrderResultStatus ResultStatus,
        Guid? OrderId,
        OrderStatus? OrderStatus,
        string? FailureReason)
    {
        public static CreateOrderResult Success(Guid orderId, OrderStatus orderStatus) =>
            new(CreateOrderResultStatus.Success, orderId, orderStatus, null);

        public static CreateOrderResult IdempotencyKeyConflict() =>
            new(CreateOrderResultStatus.IdempotencyKeyConflict, null, null, null);

        public static CreateOrderResult RequestAlreadyProcessing() =>
            new(CreateOrderResultStatus.RequestAlreadyProcessing, null, null, null);

        public static CreateOrderResult WarehouseResolveFailed(string? reason) =>
            new(CreateOrderResultStatus.WarehouseResolveFailed, null, null, reason);
    }
}
