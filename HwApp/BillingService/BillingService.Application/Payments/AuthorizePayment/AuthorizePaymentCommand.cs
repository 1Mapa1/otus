using MediatR;

namespace BillingService.Application.Payments.AuthorizePayment
{
    public sealed record AuthorizePaymentCommand(
        Guid UserId,
        Guid OrderId,
        decimal Amount) : IRequest<AuthorizePaymentResult>;
}
