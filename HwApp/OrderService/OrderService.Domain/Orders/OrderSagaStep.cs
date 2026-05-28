namespace OrderService.Domain.Orders
{
    public enum OrderSagaStep
    {
        Created = 0,
        PaymentAuthorized = 1,
        StockReserved= 2,
        DeliveryReserved = 3,
        Completed = 4,
        Compensating = 5,
        Compensated = 6
    }
}