namespace WarehouseService.Application.Reservations.Operations
{
    public sealed record ReserveProductItem(
        Guid ProductId,
        int Quantity);
}
