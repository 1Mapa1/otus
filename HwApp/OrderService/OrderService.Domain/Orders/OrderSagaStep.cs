namespace OrderService.Domain.Orders
{
    public enum OrderSagaStep
    {
        Created = 0,
        PaymentAuthorized = 1,
        StockReserved= 2,
        DeliveryReserved = 3,
        Completed = 5,
        Compensating = 6,
        Compensated = 7,
        CompensationFailed = 8,
    }
}