using DeliveryService.Application.Abstractions;
using DeliveryService.Application.Common;
using DeliveryService.Application.Zones;
using MediatR;

namespace DeliveryService.Application.Zones.UpdateDeliveryZone
{
    internal sealed class UpdateDeliveryZoneHandler
        : IRequestHandler<UpdateDeliveryZoneCommand, Result<UpdateDeliveryZoneResult>>
    {
        private static readonly Error ValidateName = new("ValidateName", "Name is required.", ErrorType.Validation);
        private static readonly Error ZoneNotFound = new("ZoneNotFound", "Delivery zone was not found.", ErrorType.NotFound);

        private readonly IDeliveryZoneRepository _zoneRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDeliveryZoneHandler(
            IDeliveryZoneRepository zoneRepository,
            IUnitOfWork unitOfWork)
        {
            _zoneRepository = zoneRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UpdateDeliveryZoneResult>> Handle(
            UpdateDeliveryZoneCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<UpdateDeliveryZoneResult>.Failure(ValidateName);

            var zone = await _zoneRepository.GetByIdAsync(request.ZoneId, cancellationToken);

            if (zone is null)
                return Result<UpdateDeliveryZoneResult>.Failure(ZoneNotFound);

            zone.Update(request.Name, request.IsActive);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<UpdateDeliveryZoneResult>.Success(
                new UpdateDeliveryZoneResult(
                    zone.Id,
                    zone.Name,
                    zone.City,
                    zone.IsActive));
        }
    }
}
