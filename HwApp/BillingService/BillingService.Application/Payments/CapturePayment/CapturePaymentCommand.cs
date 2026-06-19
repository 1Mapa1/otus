using MediatR;

namespace BillingService.Application.Payments.CapturePayment
{
    public sealed record CapturePaymentCommand(
        Guid OrderId) : IRequest<CapturePaymentResult>;
}
