using DeliveryService.Application.Common;
using DeliveryService.Application.Slots;
using DeliveryService.Application.Zones;
using MediatR;

namespace DeliveryService.Application.Slots.GetAvailableDeliverySlots
{
    internal sealed class GetAvailableDeliverySlotsHandler
        : IRequestHandler<GetAvailableDeliverySlotsQuery, Result<GetAvailableDeliverySlotsResult>>
    {
        private static readonly Error ValidateCity = new("ValidateCity", "City is required.", ErrorType.Validation);
        private static readonly Error ValidateStreet = new("ValidateStreet", "Street is required.", ErrorType.Validation);
        private static readonly Error ValidateHouse = new("ValidateHouse", "House is required.", ErrorType.Validation);
        private static readonly Error ZoneNotFound = new("ZoneNotFound", "Delivery zone was not found for the address city.", ErrorType.NotFound);

        private readonly IDeliveryZoneRepository _zoneRepository;
        private readonly IDeliverySlotRepository _slotRepository;

        public GetAvailableDeliverySlotsHandler(
            IDeliveryZoneRepository zoneRepository,
            IDeliverySlotRepository slotRepository)
        {
            _zoneRepository = zoneRepository;
            _slotRepository = slotRepository;
        }

        public async Task<Result<GetAvailableDeliverySlotsResult>> Handle(
            GetAvailableDeliverySlotsQuery request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.City))
                return Result<GetAvailableDeliverySlotsResult>.Failure(ValidateCity);

            if (string.IsNullOrWhiteSpace(request.Street))
                return Result<GetAvailableDeliverySlotsResult>.Failure(ValidateStreet);

            if (string.IsNullOrWhiteSpace(request.House))
                return Result<GetAvailableDeliverySlotsResult>.Failure(ValidateHouse);

            var zone = await _zoneRepository.GetActiveByCityAsync(request.City, cancellationToken);

            if (zone is null)
                return Result<GetAvailableDeliverySlotsResult>.Failure(ZoneNotFound);

            var slots = await _slotRepository.GetAvailableByZoneIdAsync(zone.Id, cancellationToken);

            var items = slots
                .Select(slot => new AvailableDeliverySlotDto(
                    slot.SlotId,
                    slot.TimeFrom,
                    slot.TimeTo,
                    slot.AvailableCapacity))
                .ToList();

            return Result<GetAvailableDeliverySlotsResult>.Success(
                new GetAvailableDeliverySlotsResult(items));
        }
    }
}
