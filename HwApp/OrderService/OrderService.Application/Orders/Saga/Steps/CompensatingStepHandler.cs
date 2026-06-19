using OrderService.Application.Abstractions.Clients.Billing;
using OrderService.Application.Abstractions.Clients.Delivery;
using OrderService.Application.Abstractions.Clients.Warehouse;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga.Steps
{
    internal sealed class CompensatingStepHandler : IOrderSagaStepHandler
    {
        private readonly IBillingClient _billingClient;
        private readonly IWarehouseClient _warehouseClient;
        private readonly IDeliveryClient _deliveryClient;

        public CompensatingStepHandler(IBillingClient billingClient, IWarehouseClient warehouseClient, IDeliveryClient deliveryClient) 
        {
            _billingClient = billingClient;
            _warehouseClient = warehouseClient;
            _deliveryClient = deliveryClient;
        }

        public OrderSagaStep Step => OrderSagaStep.Compensating;

        public async Task HandleAsync(Order order, CancellationToken cancellationToken)
        {
            if (order.DeliveryReservationId.HasValue)
            {
                var result = await _deliveryClient.CancelReservationAsync(order.Id, cancellationToken);

                if (!result.IsSuccess)
                {
                    order.MarkAsCompensationFailed(
                        result.Error?.Message ?? "Delivery reservation compensation failed.");

                    return;
                }
            }

            if (order.StockReservationId.HasValue)
            {
                var result = await _warehouseClient.CancelReservationAsync(order.Id, cancellationToken);

                if (!result.IsSuccess)
                {
                    order.MarkAsCompensationFailed(
                        result.Error?.Message ?? "Stock reservation compensation failed.");

                    return;
                }
            }

            if (order.PaymentId.HasValue)
            {
                var result = await _billingClient.CancelAuthorizationAsync(order.Id, cancellationToken);

                if (!result.IsSuccess)
                {
                    order.MarkAsCompensationFailed(
                        result.Error?.Message ?? "Payment compensation failed.");

                    return;
                }
            }

            order.MarkAsCompensated();
        }
    }
}
