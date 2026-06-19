namespace BillingService.Application.Payments.Operations
{
    public sealed record CaptureOperationResult(
        CaptureOperationStatus Status,
        Guid? PaymentId = null,
        decimal? CapturedAmount = null)
    {
        public static CaptureOperationResult Success(Guid paymentId, decimal CapturedAmount)
            => new(CaptureOperationStatus.Success, paymentId, CapturedAmount);

        public static CaptureOperationResult PaymentNotFound()
            => new(CaptureOperationStatus.PaymentNotFound);

        public static CaptureOperationResult InvalidPaymentState()
            => new(CaptureOperationStatus.InvalidPaymentState);

        public static CaptureOperationResult AccountStateConflict()
            => new(CaptureOperationStatus.AccountStateConflict);
    }
}
