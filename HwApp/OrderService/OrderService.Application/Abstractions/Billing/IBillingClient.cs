using OrderService.Application.Abstractions.Billing.AuthorizePayment;
using OrderService.Application.Abstractions.Billing.CancelAuthorization;
using OrderService.Application.Abstractions.Billing.CapturePayment;

namespace OrderService.Application.Abstractions.Billing
{
    public interface IBillingClient
    {
        Task<AuthorizePaymentResult> AuthorizePaymentAsync(
            Guid userId,
            Guid orderId,
            decimal amount,
            CancellationToken cancellationToken = default);

        Task<CapturePaymentResult> CapturePaymentAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<CancelAuthorizationResult> CancelAuthorizationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
