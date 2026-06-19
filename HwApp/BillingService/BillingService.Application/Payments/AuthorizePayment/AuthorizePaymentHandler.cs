using BillingService.Application.Payments.Operations;
using MediatR;

namespace BillingService.Application.Payments.AuthorizePayment
{
    public sealed class AuthorizePaymentHandler : IRequestHandler<AuthorizePaymentCommand, AuthorizePaymentResult>
    {
        private readonly IPaymentRepository _paymentRepository;

        public AuthorizePaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<AuthorizePaymentResult> Handle(AuthorizePaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                return AuthorizePaymentResult.InvalidAmount();

            var result = await _paymentRepository.AuthorizeAsync(
                request.UserId,
                request.OrderId,
                request.Amount,
                cancellationToken);

            return result.Status switch
            {
                AuthorizeOperationStatus.Success =>
                    AuthorizePaymentResult.Success(result.PaymentId!.Value, result.AuthorizedAmount!.Value),

                AuthorizeOperationStatus.AccountNotFound =>
                    AuthorizePaymentResult.AccountNotFound(),

                AuthorizeOperationStatus.InsufficientFunds =>
                    AuthorizePaymentResult.InsufficientFunds(),

                AuthorizeOperationStatus.InvalidPaymentState =>
                    AuthorizePaymentResult.InvalidPaymentState(),

                _ => throw new InvalidOperationException(
                    $"Unknown authorize operation status: {result.Status}")
            };
        }
    }
}
