namespace BillingService.Application.Payments.AuthorizePayment
{
    public enum AuthorizePaymentStatus
    {
        Success = 0,
        AccountNotFound = 1,
        InsufficientFunds = 2,
        InvalidPaymentState = 3,
        InvalidAmount = 4,
    }
}
