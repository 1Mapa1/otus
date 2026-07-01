namespace OrderService.Infrastructure.Clients.Delivery.Requests
{
    internal sealed record DeliveryAddressRequest(
        string City,
        string Street,
        string House,
        string? Apartment);

    internal sealed record CreateReservationRequest(
        Guid OrderId,
        Guid CustomerId,
        Guid DeliverySlotId,
        DeliveryAddressRequest Address);
}
