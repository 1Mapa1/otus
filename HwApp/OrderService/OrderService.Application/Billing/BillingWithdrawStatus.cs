namespace OrderService.Application.Billing
{
    public enum BillingWithdrawStatus
    {
        Success = 0,
        InsufficientFunds = 1,
        AccountNotFound = 2,
        InvalidAmount = 3,
        UnknownError = 4,
    }
}
