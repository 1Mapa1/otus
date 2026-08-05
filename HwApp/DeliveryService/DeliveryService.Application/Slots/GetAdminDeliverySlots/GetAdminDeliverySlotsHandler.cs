using DeliveryService.Application.Common;
using DeliveryService.Application.Slots;
using MediatR;

namespace DeliveryService.Application.Slots.GetAdminDeliverySlots
{
    internal sealed class GetAdminDeliverySlotsHandler
        : IRequestHandler<GetAdminDeliverySlotsQuery, Result<GetAdminDeliverySlotsResult>>
    {
        private readonly IDeliverySlotRepository _slotRepository;

        public GetAdminDeliverySlotsHandler(IDeliverySlotRepository slotRepository)
        {
            _slotRepository = slotRepository;
        }

        public async Task<Result<GetAdminDeliverySlotsResult>> Handle(
            GetAdminDeliverySlotsQuery request,
            CancellationToken cancellationToken)
        {
            var slots = await _slotRepository.GetAllForAdminAsync(cancellationToken);

            var items = slots
                .Select(slot => new AdminDeliverySlotDto(
                    slot.Id,
                    slot.ZoneId,
                    slot.Zone.Name,
                    slot.Zone.City,
                    slot.TimeFrom,
                    slot.TimeTo,
                    slot.Capacity,
                    slot.ReservedCount,
                    slot.Status.ToString()))
                .ToList();

            return Result<GetAdminDeliverySlotsResult>.Success(
                new GetAdminDeliverySlotsResult(items));
        }
    }
}
