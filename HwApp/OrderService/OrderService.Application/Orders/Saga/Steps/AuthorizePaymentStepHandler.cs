using OrderService.Application.Abstractions.Clients.Billing;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga.Steps
{
    internal sealed class AuthorizePaymentStepHandler : IOrderSagaStepHandler
    {
        private readonly IBillingClient _billingClient;

        public AuthorizePaymentStepHandler(IBillingClient billingClient)
        {
            _billingClient = billingClient;
        }

        public OrderSagaStep Step => OrderSagaStep.Created;

        public async Task HandleAsync(Order order, CancellationToken cancellationToken)
        {
            var result = await _billingClient.AuthorizePaymentAsync(order.Id, order.UserId, order.TotalAmount, cancellationToken);

            if (!result.IsSuccess)
            {
                var reason = result.Error?.Code == BillingClientErrorCode.InsufficientFunds
                    ? OrderFailureReason.InsufficientFunds
                    : OrderFailureReason.UnknownError;

                order.MarkAsRejected(
                    reason,
                    result.Error?.Message ?? "Unknown error during payment authorization.");

                return;
            }

            order.MarkAsPaymentAuthorized(result.PaymentId!.Value);
        }
    }
}
