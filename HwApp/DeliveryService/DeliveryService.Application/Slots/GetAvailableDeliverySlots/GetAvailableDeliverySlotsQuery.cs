using DeliveryService.Application.Common;
using MediatR;

namespace DeliveryService.Application.Slots.GetAvailableDeliverySlots
{
    public sealed record GetAvailableDeliverySlotsQuery(
        string City,
        string Street,
        string House,
        string? Apartment) : IRequest<Result<GetAvailableDeliverySlotsResult>>;
}
