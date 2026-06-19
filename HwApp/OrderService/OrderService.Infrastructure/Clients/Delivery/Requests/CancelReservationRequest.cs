namespace OrderService.Infrastructure.Clients.Delivery.Requests
{
    internal sealed record CancelReservationRequest(Guid OrderId);
}
