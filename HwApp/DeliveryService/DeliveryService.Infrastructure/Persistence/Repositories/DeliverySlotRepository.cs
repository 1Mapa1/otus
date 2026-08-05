using DeliveryService.Application.Slots;
using DeliveryService.Application.Slots.GetAvailableDeliverySlots;
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
            await _databaseContext.DeliverySlots.AddAsync(deliverySlot, cancellationToken);
        }

        public async Task<DeliverySlot?> GetByIdWithZoneAsync(Guid slotId, CancellationToken cancellationToken)
        {
            return await _databaseContext.DeliverySlots
                .Include(slot => slot.Zone)
                .FirstOrDefaultAsync(slot => slot.Id == slotId, cancellationToken);
        }

        public async Task<IReadOnlyList<DeliverySlot>> GetAllForAdminAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.DeliverySlots
                .AsNoTracking()
                .Include(slot => slot.Zone)
                .OrderBy(slot => slot.TimeFrom)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<AvailableDeliverySlotItem>> GetAvailableByZoneIdAsync(
            Guid zoneId,
            CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;

            return await _databaseContext.DeliverySlots
                .AsNoTracking()
                .Where(slot =>
                    slot.ZoneId == zoneId &&
                    slot.Status == DeliverySlotStatus.Open &&
                    slot.TimeFrom > utcNow &&
                    slot.ReservedCount < slot.Capacity)
                .OrderBy(slot => slot.TimeFrom)
                .Select(slot => new AvailableDeliverySlotItem(
                    slot.Id,
                    slot.TimeFrom,
                    slot.TimeTo,
                    slot.Capacity - slot.ReservedCount))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> TryUpdateCapacityAsync(
            Guid slotId,
            int newCapacity,
            CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;

            var affectedRows = await _databaseContext.DeliverySlots
                .Where(slot =>
                    slot.Id == slotId &&
                    slot.ReservedCount <= newCapacity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(slot => slot.Capacity, newCapacity)
                    .SetProperty(slot => slot.UpdatedAt, utcNow),
                    cancellationToken);

            return affectedRows == 1;
        }
    }
}
