using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.Saga
{
    public interface IOrderSagaProcessor
    {
        Task ProcessAsync(Order order, CancellationToken cancellationToken);
    }
}
