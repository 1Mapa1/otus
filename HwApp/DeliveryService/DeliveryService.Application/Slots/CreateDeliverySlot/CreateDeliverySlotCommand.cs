using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Slots.CreateDeliverySlot
{
    public sealed record CreateDeliverySlotCommand(
        DateTime TimeFrom,
        DateTime TimeTo) : IRequest<Result<CreateDeliverySlotResult>>;
}
