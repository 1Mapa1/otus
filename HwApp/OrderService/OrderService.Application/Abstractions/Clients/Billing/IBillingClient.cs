using OrderService.Application.Abstractions.Clients.Billing.AuthorizePayment;
using OrderService.Application.Abstractions.Clients.Billing.CancelAuthorization;
using OrderService.Application.Abstractions.Clients.Billing.CapturePayment;

namespace OrderService.Application.Abstractions.Clients.Billing
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
