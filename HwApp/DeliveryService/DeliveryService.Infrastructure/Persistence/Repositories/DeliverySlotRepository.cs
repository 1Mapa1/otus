using DeliveryService.Application.Slots;
using DeliveryService.Domain.Slots;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Infrastructure.Persistence.Repositories
{
    internal sealed class DeliverySlotRepository : IDeliverySlotRepository
    {
        private readonly DatabaseContext _databaseContext;

        public DeliverySlotRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task AddAsync(DeliverySlot deliverySlot, CancellationToken cancellationToken)
        {
            await _databaseContext.AddAsync(deliverySlot, cancellationToken);
        }

        public async Task<IReadOnlyList<DeliverySlot>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.DeliverySlots.ToListAsync(cancellationToken);
        }
    }
}
