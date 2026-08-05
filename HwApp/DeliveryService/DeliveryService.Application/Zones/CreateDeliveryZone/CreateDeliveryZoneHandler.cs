using DeliveryService.Application.Abstractions;
using DeliveryService.Application.Common;
using DeliveryService.Application.Zones;
using DeliveryService.Domain.Zones;
using MediatR;

namespace DeliveryService.Application.Zones.CreateDeliveryZone
{
    internal sealed class CreateDeliveryZoneHandler
        : IRequestHandler<CreateDeliveryZoneCommand, Result<CreateDeliveryZoneResult>>
    {
        private static readonly Error ValidateName = new("ValidateName", "Name is required.", ErrorType.Validation);
        private static readonly Error ValidateCity = new("ValidateCity", "City is required.", ErrorType.Validation);
        private static readonly Error CityAlreadyExists = new("CityAlreadyExists", "Delivery zone for this city already exists.", ErrorType.Conflict);

        private readonly IDeliveryZoneRepository _zoneRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDeliveryZoneHandler(
            IDeliveryZoneRepository zoneRepository,
            IUnitOfWork unitOfWork)
        {
            _zoneRepository = zoneRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateDeliveryZoneResult>> Handle(
            CreateDeliveryZoneCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<CreateDeliveryZoneResult>.Failure(ValidateName);

            if (string.IsNullOrWhiteSpace(request.City))
                return Result<CreateDeliveryZoneResult>.Failure(ValidateCity);

            if (await _zoneRepository.ExistsByCityAsync(request.City, cancellationToken))
                return Result<CreateDeliveryZoneResult>.Failure(CityAlreadyExists);

            var zone = DeliveryZone.Create(request.Name, request.City);

            await _zoneRepository.AddAsync(zone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreateDeliveryZoneResult>.Success(
                new CreateDeliveryZoneResult(
                    zone.Id,
                    zone.Name,
                    zone.City,
                    zone.IsActive));
        }
    }
}
