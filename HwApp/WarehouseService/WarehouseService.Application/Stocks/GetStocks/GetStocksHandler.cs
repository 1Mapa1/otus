using WarehouseService.Application.Common;
using WarehouseService.Application.Stocks;
using MediatR;

namespace WarehouseService.Application.Stocks.GetStocks
{
    internal sealed class GetStocksHandler : IRequestHandler<GetStocksQuery, Result<GetStocksResult>>
    {
        private readonly IStockItemRepository _stockItemRepository;

        public GetStocksHandler(IStockItemRepository stockItemRepository)
        {
            _stockItemRepository = stockItemRepository;
        }

        public async Task<Result<GetStocksResult>> Handle(
            GetStocksQuery request,
            CancellationToken cancellationToken)
        {
            var stockItems = await _stockItemRepository.GetAllAsync(cancellationToken);

            var items = stockItems
                .Select(item => new StockItemDto(
                    item.ProductId,
                    item.AvailableQuantity,
                    item.ReservedQuantity,
                    item.IsActive,
                    item.UpdatedAt))
                .ToList();

            return Result<GetStocksResult>.Success(new GetStocksResult(items));
        }
    }
}
