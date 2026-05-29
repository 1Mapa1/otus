namespace OrderService.Domain.Orders
{
    public enum OrderFailureReason
    {
        InsufficientFunds = 1,
        StockNotAvailable = 2,
        DeliverySlotUnavailable = 3,
        UnknownError = 4,
    }
}
