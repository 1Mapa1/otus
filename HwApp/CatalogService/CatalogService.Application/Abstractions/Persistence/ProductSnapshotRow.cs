namespace CatalogService.Application.Abstractions.Persistence
{
    public sealed record ProductSnapshotRow(
        Guid ProductId,
        string Name,
        decimal Price,
        bool IsActive);
}
