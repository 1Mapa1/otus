using OrderService.Application.Abstractions.Clients.Warehouse;
using OrderService.Application.Abstractions.Clients.Warehouse.CreateReservation;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga.Steps
{
    internal sealed class ReserveStockStepHandler : IOrderSagaStepHandler
    {
        private readonly IWarehouseClient _warehouseClient;

        public ReserveStockStepHandler(IWarehouseClient warehouseClient)
        {
            _warehouseClient = warehouseClient;
        }

        public OrderSagaStep Step => OrderSagaStep.PaymentAuthorized;

        public async Task HandleAsync(Order order, CancellationToken cancellationToken)
        {
            var result = await _warehouseClient.CreateReservationAsync(
                order.Id,
                order.UserId,
                order.Items.Select(i => new CreateReservationItem(
                    i.ProductId,
                    i.Quantity)).ToList(), 
                cancellationToken);

            if (!result.IsSuccess)
            {
                var reason = result.Error?.Code == WarehouseClientErrorCode.StockNotAvailable
                    ? OrderFailureReason.StockNotAvailable
                    : OrderFailureReason.UnknownError;

                order.MarkAsCompensating(
                    reason,
                    result.Error?.Message ?? "Unknown error during stock reservation.");

                return;
            }

            order.MarkAsStockReserved(result.ReservationId!.Value);
        }
    }
}
