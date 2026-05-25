namespace BillingService.Application.Payments.Operations
{
    public enum CaptureOperationStatus 
    {
        Success = 0,
        PaymentNotFound = 1,
        InvalidPaymentState = 2,
        AccountStateConflict = 3,
    }
}
