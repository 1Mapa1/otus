namespace BillingService.Application.Payments.Operations
{
    public enum AuthorizeOperationStatus
    {
        Success = 0,
        AccountNotFound = 1,
        InsufficientFunds = 2,
        InvalidPaymentState = 3,
    }
}
