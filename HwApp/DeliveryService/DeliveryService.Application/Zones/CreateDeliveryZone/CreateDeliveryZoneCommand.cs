using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Zones.CreateDeliveryZone
{
    public sealed record CreateDeliveryZoneCommand(string Name, string City)
        : IRequest<Result<CreateDeliveryZoneResult>>;
}
