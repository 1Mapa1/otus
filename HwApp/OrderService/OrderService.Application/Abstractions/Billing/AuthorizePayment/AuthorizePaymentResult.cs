namespace OrderService.Application.Abstractions.Billing.AuthorizePayment
{
    public sealed record AuthorizePaymentResult
    {
        public bool IsSuccess { get; init; }

        public Guid? PaymentId { get; init; }

        public decimal? AuthorizedAmount { get; init; }

        public BillingClientError? Error { get; init; }

        public static AuthorizePaymentResult Success(Guid paymentId, decimal authorizedAmount)
        {
            return new AuthorizePaymentResult
            {
                IsSuccess = true,
                PaymentId = paymentId,
                AuthorizedAmount = authorizedAmount
            };
        }

        public static AuthorizePaymentResult Failure(BillingClientError error)
        {
            return new AuthorizePaymentResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
