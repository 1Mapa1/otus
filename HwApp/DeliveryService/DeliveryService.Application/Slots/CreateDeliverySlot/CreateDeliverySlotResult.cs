using DeliveryService.Domain.Slots;

namespace DeliveryService.Application.Slots.CreateDeliverySlot
{
    public sealed record CreateDeliverySlotResult(
        Guid SlotId,
        DateTime TimeFrom,
        DateTime TimeTo,
        DeliverySlotStatus Status);
}
