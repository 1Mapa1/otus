using OrderService.Application.Abstractions.Clients.Delivery;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga.Steps
{
    internal sealed class ReserveDeliveryStepHandler : IOrderSagaStepHandler
    {
        private readonly IDeliveryClient _deliveryClient;

        public ReserveDeliveryStepHandler(IDeliveryClient deliveryClient)
        {
            _deliveryClient = deliveryClient;
        }

        public OrderSagaStep Step => OrderSagaStep.StockReserved;

        public async Task HandleAsync(Order order, CancellationToken cancellationToken)
        {
            var result = await _deliveryClient.CreateReservationAsync(
                order.Id,
                order.UserId,
                order.DeliverySlotId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                var reason = result.Error?.Code == DeliveryClientErrorCode.SlotNotAvailable
                    ? OrderFailureReason.DeliverySlotUnavailable
                    : OrderFailureReason.UnknownError;

                order.MarkAsCompensating(
                    reason,
                    result.Error?.Message ?? "Unknown error during delivery reservation.");

                return;
            }

            order.MarkAsDeliveryReserved(result.ReservationId!.Value);
        }
    }
}
