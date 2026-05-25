namespace BillingService.Application.Accounts.WithdrawAccount
{
    public enum WithdrawAccountStatus
    {
        Success = 1,
        InvalidAmount = 2,
        AccountNotFound = 3,
        InsufficientFunds = 4
    }
}
