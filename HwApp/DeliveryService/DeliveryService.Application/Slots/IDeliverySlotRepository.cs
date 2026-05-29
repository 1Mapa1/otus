using DeliveryService.Domain.Slots;

namespace DeliveryService.Application.Slots
{
    public interface IDeliverySlotRepository
    {
        Task AddAsync(DeliverySlot deliverySlot, CancellationToken cancellationToken);
        Task<IReadOnlyList<DeliverySlot>> GetAllAsync(CancellationToken cancellationToken);
    }
}
