using BillingService.Application.Payments.Operations;
using MediatR;

namespace BillingService.Application.Payments.CapturePayment
{
    internal sealed class CapturePaymentHandler : IRequestHandler<CapturePaymentCommand, CapturePaymentResult>
    {
        private readonly IPaymentRepository _paymentRepository;

        public CapturePaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<CapturePaymentResult> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
        {
            var result = await _paymentRepository.CaptureAsync(
                request.OrderId,
                cancellationToken);

            return result.Status switch
            {
                CaptureOperationStatus.Success =>
                    CapturePaymentResult.Success(result.PaymentId!.Value, result.CapturedAmount!.Value),

                CaptureOperationStatus.PaymentNotFound =>
                    CapturePaymentResult.PaymentNotFound(),

                CaptureOperationStatus.InvalidPaymentState =>
                    CapturePaymentResult.InvalidPaymentState(),

                CaptureOperationStatus.AccountStateConflict =>
                    CapturePaymentResult.AccountStateConflict(),

                _ => throw new InvalidOperationException(
                    $"Unknown capture operation status: {result.Status}")
            };
        }
    }
}
