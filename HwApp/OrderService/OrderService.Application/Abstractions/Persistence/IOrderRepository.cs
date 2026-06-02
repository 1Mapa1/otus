using OrderService.Domain.Orders;

namespace OrderService.Application.Abstractions.Persistence
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddAsync(Order order, CancellationToken cancellationToken = default);
    }
}
