using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Zones.GetDeliveryZones
{
    public sealed record GetDeliveryZonesQuery()
        : IRequest<Result<GetDeliveryZonesResult>>;
}
