namespace OrderService.Domain.Orders
{
    public enum OrderFailureReason
    {
        InsufficientFunds = 1,
        InvalidAmount = 2,
        AccountNotFound = 3,
        UnknownError = 4,
    }
}
