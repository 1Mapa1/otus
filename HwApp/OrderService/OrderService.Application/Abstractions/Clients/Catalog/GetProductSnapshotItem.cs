namespace OrderService.Application.Abstractions.Clients.Catalog
{
    public sealed record GetProductSnapshotItem(Guid ProductId, int Quantity);
}
