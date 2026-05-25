namespace BillingService.Application.Payments.CancelAuthorizationPayment
{
    public sealed record CancelAuthorizationPaymentResult(
        CancelAuthorizationPaymentStatus Status)
    {
        public static CancelAuthorizationPaymentResult Success()
        {
            return new CancelAuthorizationPaymentResult(CancelAuthorizationPaymentStatus.Success);
        }

        public static CancelAuthorizationPaymentResult InvalidPaymentState()
        {
            return new CancelAuthorizationPaymentResult(CancelAuthorizationPaymentStatus.InvalidPaymentState);
        }

        public static CancelAuthorizationPaymentResult AccountStateConflict()
        {
            return new CancelAuthorizationPaymentResult(CancelAuthorizationPaymentStatus.AccountStateConflict);
        }
    }
}
