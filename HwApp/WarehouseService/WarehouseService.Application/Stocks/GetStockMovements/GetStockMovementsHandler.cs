using WarehouseService.Application.Common;
using WarehouseService.Application.Stocks;
using MediatR;

namespace WarehouseService.Application.Stocks.GetStockMovements
{
    internal sealed class GetStockMovementsHandler
        : IRequestHandler<GetStockMovementsQuery, Result<GetStockMovementsResult>>
    {
        private static readonly Error StockItemNotFound = new(
            "StockItemNotFound",
            "Stock item was not found.",
            ErrorType.NotFound);

        private readonly IStockItemRepository _stockItemRepository;
        private readonly IStockMovementRepository _stockMovementRepository;

        public GetStockMovementsHandler(
            IStockItemRepository stockItemRepository,
            IStockMovementRepository stockMovementRepository)
        {
            _stockItemRepository = stockItemRepository;
            _stockMovementRepository = stockMovementRepository;
        }

        public async Task<Result<GetStockMovementsResult>> Handle(
            GetStockMovementsQuery request,
            CancellationToken cancellationToken)
        {
            var stockItem = await _stockItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

            if (stockItem is null)
                return Result<GetStockMovementsResult>.Failure(StockItemNotFound);

            var movements = await _stockMovementRepository.GetByProductIdAsync(
                request.ProductId,
                cancellationToken);

            var items = movements
                .Select(movement => new StockMovementDto(
                    movement.Id,
                    movement.ProductId,
                    movement.Type.ToString(),
                    movement.Quantity,
                    movement.OrderId,
                    movement.CreatedAt))
                .ToList();

            return Result<GetStockMovementsResult>.Success(new GetStockMovementsResult(items));
        }
    }
}
