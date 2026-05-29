namespace BillingService.Application.Payments.Operations
{
    public sealed record AuthorizeOperationResult(
        AuthorizeOperationStatus Status,
        Guid? PaymentId = null,
        decimal? AuthorizedAmount = null)
    {
        public static AuthorizeOperationResult Success(Guid paymentId, decimal authorizedAmount)
            => new(AuthorizeOperationStatus.Success, paymentId, authorizedAmount);

        public static AuthorizeOperationResult AccountNotFound()
            => new(AuthorizeOperationStatus.AccountNotFound);

        public static AuthorizeOperationResult InsufficientFunds()
            => new(AuthorizeOperationStatus.InsufficientFunds);

        public static AuthorizeOperationResult InvalidPaymentState()
        => new(AuthorizeOperationStatus.InvalidPaymentState);
    }
}
