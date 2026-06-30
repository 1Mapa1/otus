using WarehouseService.Application.Common;
using WarehouseService.Application.Stocks;
using MediatR;

namespace WarehouseService.Application.Stocks.StockIncome
{
    internal sealed class StockIncomeHandler : IRequestHandler<StockIncomeCommand, Result<StockIncomeResult>>
    {
        private static readonly Error InvalidQuantity = new(
            "InvalidQuantity",
            "Quantity must be greater than zero.",
            ErrorType.Validation);

        private static readonly Error StockItemNotFound = new(
            "StockItemNotFound",
            "Stock item was not found.",
            ErrorType.NotFound);

        private static readonly Error Conflict = new(
            "Conflict",
            "Stock item is inactive.",
            ErrorType.Conflict);

        private readonly IStockItemRepository _stockItemRepository;

        public StockIncomeHandler(IStockItemRepository stockItemRepository)
        {
            _stockItemRepository = stockItemRepository;
        }

        public async Task<Result<StockIncomeResult>> Handle(
            StockIncomeCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
                return Result<StockIncomeResult>.Failure(InvalidQuantity);

            var result = await _stockItemRepository.IncomeAsync(
                request.ProductId,
                request.Quantity,
                cancellationToken);

            return result switch
            {
                StockIncomeOperationResult.Success =>
                    await BuildSuccessAsync(request.ProductId, cancellationToken),

                StockIncomeOperationResult.StockItemNotFound =>
                    Result<StockIncomeResult>.Failure(StockItemNotFound),

                StockIncomeOperationResult.Conflict =>
                    Result<StockIncomeResult>.Failure(Conflict),

                _ => throw new InvalidOperationException($"Unknown stock income operation result: {result}")
            };
        }

        private async Task<Result<StockIncomeResult>> BuildSuccessAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var stockItem = await _stockItemRepository.GetByProductIdAsync(productId, cancellationToken);

            if (stockItem is null)
                return Result<StockIncomeResult>.Failure(StockItemNotFound);

            return Result<StockIncomeResult>.Success(
                new StockIncomeResult(
                    stockItem.ProductId,
                    stockItem.AvailableQuantity,
                    stockItem.ReservedQuantity,
                    stockItem.IsActive,
                    stockItem.UpdatedAt));
        }
    }
}
