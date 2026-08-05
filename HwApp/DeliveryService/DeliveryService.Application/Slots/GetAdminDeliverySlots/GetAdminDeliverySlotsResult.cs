namespace DeliveryService.Application.Slots.GetAdminDeliverySlots
{
    public sealed record GetAdminDeliverySlotsResult(
        IReadOnlyList<AdminDeliverySlotDto> DeliverySlots);
}
