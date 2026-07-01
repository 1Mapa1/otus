using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.GetProductSnapshot
{
    internal sealed class GetProductSnapshotHandler : IRequestHandler<GetProductSnapshotCommand, Result<GetProductSnapshotResult>>
    {
        private readonly IProductSnapshotRepository _snapshotRepository;

        public GetProductSnapshotHandler(IProductSnapshotRepository snapshotRepository)
        {
            _snapshotRepository = snapshotRepository;
        }

        public async Task<Result<GetProductSnapshotResult>> Handle(
            GetProductSnapshotCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Items is null || request.Items.Count == 0)
                return Result<GetProductSnapshotResult>.Failure(new Error("InvalidItems", "At least one item is required.", ErrorType.Validation));

            if (request.Items.Any(item => item.ProductId == Guid.Empty || item.Quantity <= 0))
                return Result<GetProductSnapshotResult>.Failure(new Error("InvalidItems", "Each item must have a product and a positive quantity.", ErrorType.Validation));

            var distinctProductIds = request.Items.Select(item => item.ProductId).Distinct().ToList();
            var products = await _snapshotRepository.GetActiveProductsAsync(distinctProductIds, cancellationToken);
            var productsById = products.ToDictionary(product => product.ProductId);

            foreach (var item in request.Items)
            {
                if (!productsById.TryGetValue(item.ProductId, out var product))
                    return Result<GetProductSnapshotResult>.Failure(new Error("ProductNotFound", "One or more products were not found.", ErrorType.NotFound));

                if (!product.IsActive)
                    return Result<GetProductSnapshotResult>.Failure(new Error("ProductInactive", "One or more products are inactive.", ErrorType.Conflict));
            }

            var priceChangedItems = new List<PriceChangedItem>();
            var reportedProductIds = new HashSet<Guid>();

            foreach (var item in request.Items)
            {
                if (item.ExpectedUnitPrice is null)
                    continue;

                var product = productsById[item.ProductId];

                if (item.ExpectedUnitPrice == product.Price || !reportedProductIds.Add(product.ProductId))
                    continue;

                priceChangedItems.Add(new PriceChangedItem(
                    product.ProductId,
                    item.ExpectedUnitPrice.Value,
                    product.Price));
            }

            if (priceChangedItems.Count > 0)
            {
                return Result<GetProductSnapshotResult>.Failure(new Error(
                    "PriceChanged",
                    "Product price has changed.",
                    ErrorType.Conflict,
                    new
                    {
                        code = "PriceChanged",
                        message = "Product price has changed.",
                        items = priceChangedItems.Select(changedItem => new
                        {
                            productId = changedItem.ProductId,
                            expectedUnitPrice = changedItem.ExpectedUnitPrice,
                            actualUnitPrice = changedItem.ActualUnitPrice
                        })
                    }));
            }

            var responseItems = request.Items
                .Select(item =>
                {
                    var product = productsById[item.ProductId];
                    var totalPrice = product.Price * item.Quantity;

                    return new GetProductSnapshotResultItem(
                        product.ProductId,
                        product.Name,
                        product.Price,
                        item.Quantity,
                        totalPrice);
                })
                .ToList();

            var totalAmount = responseItems.Sum(item => item.TotalPrice);

            return Result<GetProductSnapshotResult>.Success(new GetProductSnapshotResult(responseItems, totalAmount));
        }
    }
}
