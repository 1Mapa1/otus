namespace OrderService.Application.Abstractions.Clients.Warehouse.CreateReservation
{
    public sealed record CreateReservationItem(
        Guid ProductId,
        int Quantity);
}
