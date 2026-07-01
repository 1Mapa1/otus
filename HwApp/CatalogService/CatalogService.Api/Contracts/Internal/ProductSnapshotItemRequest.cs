namespace CatalogService.Api.Contracts.Internal
{
    public sealed record ProductSnapshotItemRequest(
        Guid ProductId,
        int Quantity,
        decimal? ExpectedUnitPrice = null);
}
