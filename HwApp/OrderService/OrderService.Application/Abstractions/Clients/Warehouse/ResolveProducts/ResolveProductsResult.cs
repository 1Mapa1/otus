using OrderService.Application.Abstractions.Clients.Warehouse;

namespace OrderService.Application.Abstractions.Clients.Warehouse.ResolveProducts
{
    public sealed record ResolveProductsResult
    {
        public bool IsSuccess { get; init; }

        public IReadOnlyList<ResolvedProductItem> Items { get; init; } = [];

        public decimal TotalAmount { get; init; }

        public WarehouseClientError? Error { get; init; }

        public static ResolveProductsResult Success(
            IReadOnlyList<ResolvedProductItem> items,
            decimal totalAmount)
        {
            return new ResolveProductsResult
            {
                IsSuccess = true,
                Items = items,
                TotalAmount = totalAmount
            };
        }

        public static ResolveProductsResult Failure(WarehouseClientError error)
        {
            return new ResolveProductsResult
            {
                IsSuccess = false,
                Error = error
            };
        }
    }
}
