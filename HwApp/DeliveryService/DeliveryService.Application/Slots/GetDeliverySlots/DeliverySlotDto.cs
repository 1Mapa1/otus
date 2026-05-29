using DeliveryService.Domain.Slots;

namespace DeliveryService.Application.Slots.GetDeliverySlots
{
    public sealed record DeliverySlotDto(
        Guid SlotId,
        DateTime TimeFrom,
        DateTime TimeTo,
        DeliverySlotStatus Status);
}