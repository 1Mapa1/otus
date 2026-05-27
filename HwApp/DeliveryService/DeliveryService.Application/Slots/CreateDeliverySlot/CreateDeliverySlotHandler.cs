using DeliveryService.Application.Abstractions;
using DeliveryService.Application.Common;
using DeliveryService.Domain.Slots;
using MediatR;

namespace DeliveryService.Application.Slots.CreateDeliverySlot
{
    internal sealed class CreateDeliverySlotHandler : IRequestHandler<CreateDeliverySlotCommand, Result<CreateDeliverySlotResult>>
    {
        private static readonly Error ValidateTime = new("ValidateTime", "TimeFrom must be earlier than TimeTo.", ErrorType.Validation);

        private readonly IDeliverySlotRepository _deliverySlotRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDeliverySlotHandler(IDeliverySlotRepository deliverySlotRepository, IUnitOfWork unitOfWork)
        {
            _deliverySlotRepository = deliverySlotRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateDeliverySlotResult>> Handle(CreateDeliverySlotCommand request, CancellationToken cancellationToken)
        {
            if (request.TimeFrom >= request.TimeTo)
                return Result<CreateDeliverySlotResult>.Failure(ValidateTime);

            var deliverySlot = DeliverySlot.Create(request.TimeFrom, request.TimeTo);

            await _deliverySlotRepository.AddAsync(deliverySlot, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreateDeliverySlotResult>.Success(new CreateDeliverySlotResult(
                deliverySlot.Id,
                deliverySlot.TimeFrom,
                deliverySlot.TimeTo,
                deliverySlot.Status));
        }
    }
}
