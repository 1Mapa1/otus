namespace BillingService.Application.Payments.CancelAuthorizationPayment
{
    public enum CancelAuthorizationPaymentStatus
    {
        Success = 0,
        InvalidPaymentState = 2,
        AccountStateConflict = 3
    }
}
