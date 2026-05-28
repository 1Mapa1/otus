namespace OrderService.Application.Abstractions.Warehouse.CreateReservation
{
    public sealed record CreateReservationItem(
        Guid ProductId,
        int Quantity);
}
