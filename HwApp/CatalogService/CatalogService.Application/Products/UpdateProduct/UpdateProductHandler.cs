using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using CatalogService.Application.Products.Validation;
using MediatR;

namespace CatalogService.Application.Products.UpdateProduct
{
    internal sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly IProductWriteRepository _productRepository;
        private readonly IBrandWriteRepository _brandRepository;
        private readonly ICategoryWriteRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductHandler(
            IProductWriteRepository productRepository,
            IBrandWriteRepository brandRepository,
            ICategoryWriteRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var validationError = ProductInputValidator.Validate(
                request.Name,
                request.Description,
                request.Price,
                request.ImageUrl,
                request.Attributes);

            if (validationError is not null)
                return Result.Failure(validationError);

            var product = await _productRepository.GetByIdWithAttributesAsync(request.ProductId, cancellationToken);
            if (product is null)
                return Result.Failure(new Error("ProductNotFound", "Product was not found.", ErrorType.NotFound));

            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
            if (brand is null)
                return Result.Failure(new Error("BrandNotFound", "Brand was not found.", ErrorType.NotFound));

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category is null)
                return Result.Failure(new Error("CategoryNotFound", "Category was not found.", ErrorType.NotFound));

            var attributeTuples = request.Attributes
                .Select(attribute => (attribute.Name, attribute.Value))
                .ToList();

            product.Update(
                request.Name,
                request.Description,
                request.BrandId,
                request.CategoryId,
                request.Price,
                request.ImageUrl,
                attributeTuples,
                DateTime.UtcNow);

            var attributeValuesJson = ProductAttributeJsonBuilder.BuildValues(attributeTuples);

            await _productRepository.UpdateAsync(product, brand.Name, attributeValuesJson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
