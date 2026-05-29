using OrderService.Application.Abstractions.Clients.Billing;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga.Steps
{
    internal sealed class CapturePaymentStepHandler : IOrderSagaStepHandler
    {
        private readonly IBillingClient _billingClient;

        public CapturePaymentStepHandler(IBillingClient billingClient)
        {
            _billingClient = billingClient;
        }

        public OrderSagaStep Step => OrderSagaStep.DeliveryReserved;

        public async Task HandleAsync(Order order, CancellationToken cancellationToken)
        {
            var result = await _billingClient.CapturePaymentAsync(order.Id, cancellationToken);

            if (!result.IsSuccess)
            {
                order.MarkAsCompensating(OrderFailureReason.UnknownError, result.Error?.Message ?? "Unknown error during payment capture.");

                return;
            }

            order.MarkAsConfirmed();
        }
    }
}
