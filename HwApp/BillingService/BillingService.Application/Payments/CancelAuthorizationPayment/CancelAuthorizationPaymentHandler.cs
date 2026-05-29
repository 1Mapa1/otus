using BillingService.Application.Payments.Operations;
using MediatR;

namespace BillingService.Application.Payments.CancelAuthorizationPayment
{
    internal sealed class CancelAuthorizationPaymentHandler : IRequestHandler<CancelAuthorizationPaymentCommand, CancelAuthorizationPaymentResult>
    {
        private readonly IPaymentRepository _paymentRepository;

        public CancelAuthorizationPaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<CancelAuthorizationPaymentResult> Handle(CancelAuthorizationPaymentCommand request, CancellationToken cancellationToken)
        {
            var result = await _paymentRepository.CancelAuthorizationAsync(
                request.OrderId,
                cancellationToken);

            return result switch
            {
                CancelAuthorizationOperationResult.Success or
                CancelAuthorizationOperationResult.PaymentNotFound =>
                    // Compensation is idempotent: if there is no authorization,
                    // there is nothing to cancel.
                    CancelAuthorizationPaymentResult.Success(),


                CancelAuthorizationOperationResult.InvalidPaymentState =>
                    CancelAuthorizationPaymentResult.InvalidPaymentState(),

                CancelAuthorizationOperationResult.AccountStateConflict =>
                    CancelAuthorizationPaymentResult.AccountStateConflict(),

                _ => throw new InvalidOperationException(
                    $"Unknown cancel authorization operation status: {result}")
            };
        }
    }
}
