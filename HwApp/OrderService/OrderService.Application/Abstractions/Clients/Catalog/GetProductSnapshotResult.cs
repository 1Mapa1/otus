namespace OrderService.Application.Abstractions.Clients.Catalog
{
    public sealed record GetProductSnapshotResult
    {
        public bool IsSuccess { get; init; }

        public IReadOnlyList<CatalogSnapshotItem> Items { get; init; } = [];

        public decimal TotalAmount { get; init; }

        public CatalogClientError? Error { get; init; }

        public static GetProductSnapshotResult Success(
            IReadOnlyList<CatalogSnapshotItem> items,
            decimal totalAmount)
        {
            return new GetProductSnapshotResult
            {
                IsSuccess = true,
                Items = items,
                TotalAmount = totalAmount
            };
        }

        public static GetProductSnapshotResult Failure(CatalogClientError error)
        {
            return new GetProductSnapshotResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
