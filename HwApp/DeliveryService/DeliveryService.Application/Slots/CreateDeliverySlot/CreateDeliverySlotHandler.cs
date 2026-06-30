using DeliveryService.Application.Abstractions;
using DeliveryService.Application.Common;
using DeliveryService.Application.Slots;
using DeliveryService.Application.Zones;
using DeliveryService.Domain.Slots;
using MediatR;

namespace DeliveryService.Application.Slots.CreateDeliverySlot
{
    internal sealed class CreateDeliverySlotHandler
        : IRequestHandler<CreateDeliverySlotCommand, Result<CreateDeliverySlotResult>>
    {
        private static readonly Error ValidateTime = new("ValidateTime", "TimeFrom must be earlier than TimeTo.", ErrorType.Validation);
        private static readonly Error ValidateCapacity = new("ValidateCapacity", "Capacity must be greater than zero.", ErrorType.Validation);
        private static readonly Error ValidateFuture = new("ValidateFuture", "Delivery slot must start in the future.", ErrorType.Validation);
        private static readonly Error ZoneNotFound = new("ZoneNotFound", "Delivery zone was not found.", ErrorType.NotFound);

        private readonly IDeliveryZoneRepository _zoneRepository;
        private readonly IDeliverySlotRepository _slotRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDeliverySlotHandler(
            IDeliveryZoneRepository zoneRepository,
            IDeliverySlotRepository slotRepository,
            IUnitOfWork unitOfWork)
        {
            _zoneRepository = zoneRepository;
            _slotRepository = slotRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateDeliverySlotResult>> Handle(
            CreateDeliverySlotCommand request,
            CancellationToken cancellationToken)
        {
            if (request.TimeFrom >= request.TimeTo)
                return Result<CreateDeliverySlotResult>.Failure(ValidateTime);

            if (request.Capacity <= 0)
                return Result<CreateDeliverySlotResult>.Failure(ValidateCapacity);

            if (request.TimeFrom <= DateTime.UtcNow)
                return Result<CreateDeliverySlotResult>.Failure(ValidateFuture);

            var zone = await _zoneRepository.GetByIdAsync(request.ZoneId, cancellationToken);

            if (zone is null)
                return Result<CreateDeliverySlotResult>.Failure(ZoneNotFound);

            var deliverySlot = DeliverySlot.Create(
                request.ZoneId,
                request.TimeFrom,
                request.TimeTo,
                request.Capacity);

            await _slotRepository.AddAsync(deliverySlot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreateDeliverySlotResult>.Success(
                new CreateDeliverySlotResult(
                    deliverySlot.Id,
                    deliverySlot.ZoneId,
                    deliverySlot.TimeFrom,
                    deliverySlot.TimeTo,
                    deliverySlot.Capacity,
                    deliverySlot.ReservedCount,
                    deliverySlot.Status.ToString()));
        }
    }
}
