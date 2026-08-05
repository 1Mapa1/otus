using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.GetProductSnapshot
{
    public sealed record GetProductSnapshotCommand(IReadOnlyList<GetProductSnapshotItem> Items)
        : IRequest<Result<GetProductSnapshotResult>>;
}
