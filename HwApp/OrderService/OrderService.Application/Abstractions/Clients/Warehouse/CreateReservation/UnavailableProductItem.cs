namespace OrderService.Application.Abstractions.Clients.Warehouse.CreateReservation
{
    public sealed record UnavailableProductItem(
        Guid ProductId,
        int RequestedQuantity,
        int FreeQuantity);
}
