namespace OrderService.Domain.Orders
{
    public enum OrderStatus
    {
        Processing = 0,
        Confirmed = 1,
        Rejected = 2,
        CompensationFailed = 3,
    }
}
