using BillingService.Application.Payments.Operations;
using BillingService.Domain.Payments;

namespace BillingService.Application.Payments
{
    public interface IPaymentRepository
    {
        Task<AuthorizeOperationResult> AuthorizeAsync(Guid userId, Guid orderId, decimal amount, CancellationToken cancellationToken);

        Task<CaptureOperationResult> CaptureAsync(Guid orderId, CancellationToken cancellationToken);

        Task<CancelAuthorizationOperationResult> CancelAuthorizationAsync(Guid orderId, CancellationToken cancellationToken);
    }
}
