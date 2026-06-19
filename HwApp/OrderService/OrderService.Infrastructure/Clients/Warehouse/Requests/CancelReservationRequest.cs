namespace OrderService.Infrastructure.Clients.Warehouse.Requests
{
    internal sealed record CancelReservationRequest(Guid OrderId);
}
