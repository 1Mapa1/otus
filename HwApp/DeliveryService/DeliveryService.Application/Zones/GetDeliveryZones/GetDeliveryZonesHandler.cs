using DeliveryService.Application.Common;
using DeliveryService.Application.Zones;
using MediatR;

namespace DeliveryService.Application.Zones.GetDeliveryZones
{
    internal sealed class GetDeliveryZonesHandler
        : IRequestHandler<GetDeliveryZonesQuery, Result<GetDeliveryZonesResult>>
    {
        private readonly IDeliveryZoneRepository _zoneRepository;

        public GetDeliveryZonesHandler(IDeliveryZoneRepository zoneRepository)
        {
            _zoneRepository = zoneRepository;
        }

        public async Task<Result<GetDeliveryZonesResult>> Handle(
            GetDeliveryZonesQuery request,
            CancellationToken cancellationToken)
        {
            var zones = await _zoneRepository.GetAllAsync(cancellationToken);

            var items = zones
                .Select(zone => new DeliveryZoneDto(
                    zone.Id,
                    zone.Name,
                    zone.City,
                    zone.IsActive))
                .ToList();

            return Result<GetDeliveryZonesResult>.Success(new GetDeliveryZonesResult(items));
        }
    }
}
