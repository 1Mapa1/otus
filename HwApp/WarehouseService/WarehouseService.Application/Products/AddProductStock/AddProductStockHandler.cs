using MediatR;
using WarehouseService.Application.Abstractions;
using WarehouseService.Application.Common;

namespace WarehouseService.Application.Products.AddProductStock
{
    public sealed class AddProductStockHandler : IRequestHandler<AddProductStockCommand, Result<AddProductStockResult>>
    {
        private static readonly Error ProductNotFound = new("ProductNotFound", "Product not found.", ErrorType.NotFound);
        private static readonly Error InvalidQuantity = new("InvalidQuantity", "Quantity must be greater than zero.", ErrorType.Validation);

        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductStockHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddProductStockResult>> Handle(AddProductStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product is null)
                return Result<AddProductStockResult>.Failure(ProductNotFound);

            if (request.Quantity <= 0)
                return Result<AddProductStockResult>.Failure(InvalidQuantity);

            product.IncreaseAvailableQuantity((uint)request.Quantity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AddProductStockResult>.Success(
                new AddProductStockResult(
                    product.Id,
                    product.AvailableQuantity,
                    product.ReservedQuantity,
                    product.FreeQuantity));
        }
    }
}
