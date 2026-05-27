using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Slots.GetDeliverySlots
{
    public sealed record GetDeliverySlotsQuery : IRequest<Result<GetDeliverySlotsResult>>;
}
