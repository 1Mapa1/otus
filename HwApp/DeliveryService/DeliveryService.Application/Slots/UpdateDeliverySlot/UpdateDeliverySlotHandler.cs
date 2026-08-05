using DeliveryService.Application.Abstractions;
using DeliveryService.Application.Common;
using DeliveryService.Application.Slots;
using DeliveryService.Application.Zones;
using DeliveryService.Domain.Slots;
using MediatR;

namespace DeliveryService.Application.Slots.UpdateDeliverySlot
{
    internal sealed class UpdateDeliverySlotHandler
        : IRequestHandler<UpdateDeliverySlotCommand, Result<UpdateDeliverySlotResult>>
    {
        private static readonly Error ValidateTime = new("ValidateTime", "TimeFrom must be earlier than TimeTo.", ErrorType.Validation);
        private static readonly Error ValidateCapacity = new("ValidateCapacity", "Capacity must be greater than zero.", ErrorType.Validation);
        private static readonly Error ValidateStatus = new("ValidateStatus", "Status is invalid.", ErrorType.Validation);
        private static readonly Error ValidateFuture = new("ValidateFuture", "Delivery slot must start in the future.", ErrorType.Validation);
        private static readonly Error SlotNotFound = new("SlotNotFound", "Delivery slot was not found.", ErrorType.NotFound);
        private static readonly Error ZoneNotFound = new("ZoneNotFound", "Delivery zone was not found.", ErrorType.NotFound);
        private static readonly Error BusinessRuleViolation = new("BusinessRuleViolation", "Delivery slot update violates business rules.", ErrorType.Conflict);

        private readonly IDeliverySlotRepository _slotRepository;
        private readonly IDeliveryZoneRepository _zoneRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDeliverySlotHandler(
            IDeliverySlotRepository slotRepository,
            IDeliveryZoneRepository zoneRepository,
            IUnitOfWork unitOfWork)
        {
            _slotRepository = slotRepository;
            _zoneRepository = zoneRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UpdateDeliverySlotResult>> Handle(
            UpdateDeliverySlotCommand request,
            CancellationToken cancellationToken)
        {
            if (request.TimeFrom >= request.TimeTo)
                return Result<UpdateDeliverySlotResult>.Failure(ValidateTime);

            if (request.Capacity <= 0)
                return Result<UpdateDeliverySlotResult>.Failure(ValidateCapacity);

            if (!Enum.TryParse<DeliverySlotStatus>(request.Status, ignoreCase: true, out var targetStatus))
                return Result<UpdateDeliverySlotResult>.Failure(ValidateStatus);

            var slot = await _slotRepository.GetByIdWithZoneAsync(request.SlotId, cancellationToken);

            if (slot is null)
                return Result<UpdateDeliverySlotResult>.Failure(SlotNotFound);

            var utcNow = DateTime.UtcNow;

            return slot.Status switch
            {
                DeliverySlotStatus.Draft => await UpdateDraftAsync(slot, request, targetStatus, utcNow, cancellationToken),
                DeliverySlotStatus.Open => await UpdateOpenAsync(slot, request, targetStatus, cancellationToken),
                DeliverySlotStatus.Closed => await UpdateClosedAsync(slot, request, targetStatus, utcNow, cancellationToken),
                _ => Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation)
            };
        }

        private async Task<Result<UpdateDeliverySlotResult>> UpdateDraftAsync(
            DeliverySlot slot,
            UpdateDeliverySlotCommand request,
            DeliverySlotStatus targetStatus,
            DateTime utcNow,
            CancellationToken cancellationToken)
        {
            var zone = await _zoneRepository.GetByIdAsync(request.ZoneId, cancellationToken);

            if (zone is null)
                return Result<UpdateDeliverySlotResult>.Failure(ZoneNotFound);

            if (request.TimeFrom <= utcNow)
                return Result<UpdateDeliverySlotResult>.Failure(ValidateFuture);

            if (request.Capacity < slot.ReservedCount)
                return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);

            if (targetStatus == DeliverySlotStatus.Open && !zone.IsActive)
                return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);

            slot.UpdateDraft(
                request.ZoneId,
                request.TimeFrom,
                request.TimeTo,
                request.Capacity,
                targetStatus);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Success(slot);
        }

        private async Task<Result<UpdateDeliverySlotResult>> UpdateOpenAsync(
            DeliverySlot slot,
            UpdateDeliverySlotCommand request,
            DeliverySlotStatus targetStatus,
            CancellationToken cancellationToken)
        {
            if (request.ZoneId != slot.ZoneId ||
                request.TimeFrom != slot.TimeFrom ||
                request.TimeTo != slot.TimeTo)
            {
                return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);
            }

            if (request.Capacity != slot.Capacity)
            {
                if (!await _slotRepository.TryUpdateCapacityAsync(slot.Id, request.Capacity, cancellationToken))
                    return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);

                slot = (await _slotRepository.GetByIdWithZoneAsync(slot.Id, cancellationToken))!;
            }

            if (targetStatus != slot.Status)
            {
                if (targetStatus != DeliverySlotStatus.Closed)
                    return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);

                slot.UpdateOpen(slot.Capacity, DeliverySlotStatus.Closed);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Success(slot);
        }

        private async Task<Result<UpdateDeliverySlotResult>> UpdateClosedAsync(
            DeliverySlot slot,
            UpdateDeliverySlotCommand request,
            DeliverySlotStatus targetStatus,
            DateTime utcNow,
            CancellationToken cancellationToken)
        {
            if (request.ZoneId != slot.ZoneId ||
                request.TimeFrom != slot.TimeFrom ||
                request.TimeTo != slot.TimeTo)
            {
                return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);
            }

            if (request.Capacity != slot.Capacity)
            {
                if (!await _slotRepository.TryUpdateCapacityAsync(slot.Id, request.Capacity, cancellationToken))
                    return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);

                slot = (await _slotRepository.GetByIdWithZoneAsync(slot.Id, cancellationToken))!;
            }

            if (targetStatus == DeliverySlotStatus.Open)
            {
                if (slot.TimeFrom <= utcNow || !slot.Zone.IsActive)
                    return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);

                slot.UpdateClosed(DeliverySlotStatus.Open, slot.Capacity);
            }
            else if (targetStatus != DeliverySlotStatus.Closed)
            {
                return Result<UpdateDeliverySlotResult>.Failure(BusinessRuleViolation);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Success(slot);
        }

        private static Result<UpdateDeliverySlotResult> Success(DeliverySlot slot)
        {
            return Result<UpdateDeliverySlotResult>.Success(
                new UpdateDeliverySlotResult(
                    slot.Id,
                    slot.ZoneId,
                    slot.TimeFrom,
                    slot.TimeTo,
                    slot.Capacity,
                    slot.ReservedCount,
                    slot.Status.ToString()));
        }
    }
}
