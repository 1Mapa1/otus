namespace OrderService.Infrastructure.Clients.Delivery.Requests
{
    internal sealed record CreateReservationRequest(
        Guid OrderId,
        Guid UserId,
        Guid DeliverySlotId);
}
