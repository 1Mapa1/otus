using OrderService.Application.Abstractions.Clients.Billing;

namespace OrderService.Application.Abstractions.Clients.Billing.CancelAuthorization
{
    public sealed record CancelAuthorizationResult
    {
        public bool IsSuccess { get; init; }

        public BillingClientError? Error { get; init; }

        public static CancelAuthorizationResult Success()
        {
            return new CancelAuthorizationResult { IsSuccess = true };
        }

        public static CancelAuthorizationResult Failure(BillingClientError error)
        {
            return new CancelAuthorizationResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
