namespace WarehouseService.Application.Reservations.CreateReservation
{
    public sealed record CreateReservationItem(
        Guid ProductId,
        int Quantity);
}
