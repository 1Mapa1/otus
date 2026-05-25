namespace BillingService.Application.Payments.CapturePayment
{
    public enum CapturePaymentStatus
    {
        Success = 0,
        PaymentNotFound = 1,
        InvalidPaymentState = 2,
        AccountStateConflict = 3,
    }
}
