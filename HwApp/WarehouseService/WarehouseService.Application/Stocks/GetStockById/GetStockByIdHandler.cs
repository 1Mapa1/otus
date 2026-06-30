using WarehouseService.Application.Common;
using WarehouseService.Application.Stocks;
using MediatR;

namespace WarehouseService.Application.Stocks.GetStockById
{
    internal sealed class GetStockByIdHandler : IRequestHandler<GetStockByIdQuery, Result<GetStockByIdResult>>
    {
        private static readonly Error StockItemNotFound = new(
            "StockItemNotFound",
            "Stock item was not found.",
            ErrorType.NotFound);

        private readonly IStockItemRepository _stockItemRepository;

        public GetStockByIdHandler(IStockItemRepository stockItemRepository)
        {
            _stockItemRepository = stockItemRepository;
        }

        public async Task<Result<GetStockByIdResult>> Handle(
            GetStockByIdQuery request,
            CancellationToken cancellationToken)
        {
            var stockItem = await _stockItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

            if (stockItem is null)
                return Result<GetStockByIdResult>.Failure(StockItemNotFound);

            return Result<GetStockByIdResult>.Success(
                new GetStockByIdResult(
                    stockItem.ProductId,
                    stockItem.AvailableQuantity,
                    stockItem.ReservedQuantity,
                    stockItem.IsActive,
                    stockItem.UpdatedAt));
        }
    }
}
