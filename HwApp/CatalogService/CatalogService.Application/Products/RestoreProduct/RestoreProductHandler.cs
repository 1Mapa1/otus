using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Products.RestoreProduct
{
    internal sealed class RestoreProductHandler : IRequestHandler<RestoreProductCommand, Result>
    {
        private readonly IProductWriteRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RestoreProductHandler(IProductWriteRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RestoreProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdWithAttributesAsync(request.ProductId, cancellationToken);
            if (product is null)
                return Result.Failure(new Error("ProductNotFound", "Product was not found.", ErrorType.NotFound));

            if (product.IsActive)
                return Result.Success();

            product.Restore(DateTime.UtcNow);
            await _productRepository.RestoreAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
