namespace BillingService.Application.Accounts
{
    public enum WithdrawOperationResult
    {
        Success = 1,
        AccountNotFound = 2,
        InsufficientFunds = 3
    }
}
