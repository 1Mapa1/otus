using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga.Steps
{
    internal interface IOrderSagaStepHandler
    {
        OrderSagaStep Step { get; }

        Task HandleAsync(Order order, CancellationToken cancellationToken);
    }
}
