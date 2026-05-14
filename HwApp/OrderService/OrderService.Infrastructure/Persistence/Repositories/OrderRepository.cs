using Microsoft.EntityFrameworkCore;
using OrderService.Application.Orders;
using OrderService.Domain.Orders;

namespace OrderService.Infrastructure.Persistence.Repositories
{
    internal sealed class OrderRepository : IOrderRepository
    {
        private readonly DatabaseContext _db;

        public OrderRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyList<Order>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var list = await _db.Orders
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return list;
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _db.Orders.AddAsync(order, cancellationToken);
        }
    }
}
