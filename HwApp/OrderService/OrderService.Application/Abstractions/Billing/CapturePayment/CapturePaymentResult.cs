namespace OrderService.Application.Abstractions.Billing.CapturePayment
{
    public sealed record CapturePaymentResult
    {
        public bool IsSuccess { get; init; }

        public Guid? PaymentId { get; init; }

        public decimal? CapturedAmount { get; init; }

        public BillingClientError? Error { get; init; }

        public static CapturePaymentResult Success(Guid paymentId, decimal capturedAmount)
        {
            return new CapturePaymentResult
            {
                IsSuccess = true,
                PaymentId = paymentId,
                CapturedAmount = capturedAmount
            };
        }

        public static CapturePaymentResult Failure(BillingClientError error)
        {
            return new CapturePaymentResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
