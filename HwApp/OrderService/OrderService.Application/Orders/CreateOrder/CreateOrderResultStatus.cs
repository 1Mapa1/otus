namespace OrderService.Application.Orders.CreateOrder
{
    public enum CreateOrderResultStatus
    {
        Success = 0,
        IdempotencyKeyConflict = 1,
        RequestAlreadyProcessing = 2,
        WarehouseResolveFailed = 3
    }
}
