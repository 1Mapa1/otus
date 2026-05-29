using MediatR;

namespace BillingService.Application.Payments.CancelAuthorizationPayment
{
    public sealed record CancelAuthorizationPaymentCommand(
        Guid OrderId) : IRequest<CancelAuthorizationPaymentResult>;
}
