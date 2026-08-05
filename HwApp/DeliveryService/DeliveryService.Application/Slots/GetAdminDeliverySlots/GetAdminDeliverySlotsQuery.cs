using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Slots.GetAdminDeliverySlots
{
    public sealed record GetAdminDeliverySlotsQuery()
        : IRequest<Result<GetAdminDeliverySlotsResult>>;
}
