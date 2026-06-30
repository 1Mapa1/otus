using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Slots.CreateDeliverySlot
{
    public sealed record CreateDeliverySlotCommand(
        Guid ZoneId,
        DateTime TimeFrom,
        DateTime TimeTo,
        int Capacity) : IRequest<Result<CreateDeliverySlotResult>>;
}
