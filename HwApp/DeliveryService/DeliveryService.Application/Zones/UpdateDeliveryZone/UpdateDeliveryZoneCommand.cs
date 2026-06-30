using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Zones.UpdateDeliveryZone
{
    public sealed record UpdateDeliveryZoneCommand(Guid ZoneId, string Name, bool IsActive)
        : IRequest<Result<UpdateDeliveryZoneResult>>;
}
