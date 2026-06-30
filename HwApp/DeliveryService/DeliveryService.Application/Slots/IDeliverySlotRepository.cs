using DeliveryService.Application.Slots.GetAvailableDeliverySlots;
using DeliveryService.Domain.Slots;

namespace DeliveryService.Application.Slots
{
    public interface IDeliverySlotRepository
    {
        Task AddAsync(DeliverySlot deliverySlot, CancellationToken cancellationToken);

        Task<DeliverySlot?> GetByIdWithZoneAsync(Guid slotId, CancellationToken cancellationToken);

        Task<IReadOnlyList<DeliverySlot>> GetAllForAdminAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<AvailableDeliverySlotItem>> GetAvailableByZoneIdAsync(
            Guid zoneId,
            CancellationToken cancellationToken);

        Task<bool> TryUpdateCapacityAsync(
            Guid slotId,
            int newCapacity,
            CancellationToken cancellationToken);
    }
}
