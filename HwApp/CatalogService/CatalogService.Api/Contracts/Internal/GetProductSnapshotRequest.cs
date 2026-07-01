namespace CatalogService.Api.Contracts.Internal
{
    public sealed record GetProductSnapshotRequest(
        IReadOnlyList<ProductSnapshotItemRequest> Items);
}
