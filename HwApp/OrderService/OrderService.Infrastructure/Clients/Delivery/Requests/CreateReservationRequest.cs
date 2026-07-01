namespace OrderService.Infrastructure.Clients.Delivery.Requests
{
    internal sealed record CreateReservationRequest(
        Guid OrderId,
        Guid CustomerId,
        Guid DeliverySlotId,
        DeliveryAddressRequest Address);
}
