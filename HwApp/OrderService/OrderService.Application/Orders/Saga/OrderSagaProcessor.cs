using OrderService.Application.Orders.Saga.Steps;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga
{
    internal sealed class OrderSagaProcessor : IOrderSagaProcessor
    {
        private readonly IReadOnlyDictionary<OrderSagaStep, IOrderSagaStepHandler> _stepHandlers;

        public OrderSagaProcessor(IEnumerable<IOrderSagaStepHandler> stepHandlers)
        {
            _stepHandlers = stepHandlers.ToDictionary(h => h.Step);
        }

        public async Task ProcessAsync(Order order, CancellationToken cancellationToken)
        {
            if (!_stepHandlers.TryGetValue(order.SagaStep, out var handler))
                return;

            await handler.HandleAsync(order, cancellationToken);
        }
    }
}
