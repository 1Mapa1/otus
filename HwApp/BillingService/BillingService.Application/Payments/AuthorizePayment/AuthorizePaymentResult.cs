namespace BillingService.Application.Payments.AuthorizePayment
{
    public sealed record AuthorizePaymentResult(
        AuthorizePaymentStatus Status,
        Guid? PaymentId = null,
        decimal? AuthorizedAmount = null)
    {
        public static AuthorizePaymentResult Success(Guid paymentId, decimal authorizedAmount)
        {
            return new AuthorizePaymentResult(AuthorizePaymentStatus.Success, paymentId, authorizedAmount);
        }

        public static AuthorizePaymentResult AccountNotFound()
        {
            return new AuthorizePaymentResult(AuthorizePaymentStatus.AccountNotFound);
        }

        public static AuthorizePaymentResult InsufficientFunds()
        {
            return new AuthorizePaymentResult(AuthorizePaymentStatus.InsufficientFunds);
        }

        public static AuthorizePaymentResult InvalidPaymentState()
        {
            return new AuthorizePaymentResult(AuthorizePaymentStatus.InvalidPaymentState);
        }

        public static AuthorizePaymentResult InvalidAmount()
        {
            return new AuthorizePaymentResult(AuthorizePaymentStatus.InvalidAmount);
        }
    }
}
