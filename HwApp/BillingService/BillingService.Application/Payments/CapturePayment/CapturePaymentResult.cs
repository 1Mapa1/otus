namespace BillingService.Application.Payments.CapturePayment
{
    public sealed record CapturePaymentResult(
        CapturePaymentStatus Status,
        Guid? PaymentId = null,
        decimal? CapturedAmount = null)
    {
        public static CapturePaymentResult Success(Guid paymentId, decimal capturedAmount)
        {
            return new CapturePaymentResult(CapturePaymentStatus.Success, paymentId, capturedAmount);
        }

        public static CapturePaymentResult PaymentNotFound()
        {
            return new CapturePaymentResult(CapturePaymentStatus.PaymentNotFound);
        }

        public static CapturePaymentResult InvalidPaymentState()
        {
            return new CapturePaymentResult(CapturePaymentStatus.InvalidPaymentState);
        }

        public static CapturePaymentResult AccountStateConflict()
        {
            return new CapturePaymentResult(CapturePaymentStatus.AccountStateConflict);
        }
    }
}
