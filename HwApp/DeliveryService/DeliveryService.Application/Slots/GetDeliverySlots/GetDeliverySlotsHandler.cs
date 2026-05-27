using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Slots.GetDeliverySlots
{
    internal sealed class GetDeliverySlotsHandler : IRequestHandler<GetDeliverySlotsQuery, Result<GetDeliverySlotsResult>>
    {
        private readonly IDeliverySlotRepository _deliverySlotRepository;

        public GetDeliverySlotsHandler(IDeliverySlotRepository deliverySlotRepository)
        {
            _deliverySlotRepository = deliverySlotRepository;
        }

        public async Task<Result<GetDeliverySlotsResult>> Handle(GetDeliverySlotsQuery request, CancellationToken cancellationToken)
        {
            var deliverySlots = await _deliverySlotRepository.GetAllAsync(cancellationToken);

            var deliverySlotDtos = deliverySlots.Select(slot => new DeliverySlotDto(
                slot.Id,
                slot.TimeFrom,
                slot.TimeTo,
                slot.Status)).ToList();

            return Result<GetDeliverySlotsResult>.Success(new GetDeliverySlotsResult(deliverySlotDtos));
        }
    }
}
