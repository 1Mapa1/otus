using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Slots.UpdateDeliverySlot
{
    public sealed record UpdateDeliverySlotCommand(
        Guid SlotId,
        Guid ZoneId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int Capacity,
        string Status) : IRequest<Result<UpdateDeliverySlotResult>>;
}
