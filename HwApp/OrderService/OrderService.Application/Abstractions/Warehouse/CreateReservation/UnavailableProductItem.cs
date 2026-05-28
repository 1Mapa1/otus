namespace OrderService.Application.Abstractions.Warehouse.CreateReservation
{
    public sealed record UnavailableProductItem(
        Guid ProductId,
        int RequestedQuantity,
        int FreeQuantity);
}
