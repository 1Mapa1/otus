using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using CatalogService.Application.Products.Validation;
using CatalogService.Domain.Products;
using MediatR;

namespace CatalogService.Application.Products.CreateProduct
{
    internal sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResult>>
    {
        private readonly IProductWriteRepository _productRepository;
        private readonly IBrandWriteRepository _brandRepository;
        private readonly ICategoryWriteRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductHandler(
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

        public async Task<Result<CreateProductResult>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var validationError = ProductInputValidator.Validate(
                request.Name,
                request.Description,
                request.Price,
                request.ImageUrl,
                request.Attributes);

            if (validationError is not null)
                return Result<CreateProductResult>.Failure(validationError);

            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
            if (brand is null)
                return Result<CreateProductResult>.Failure(new Error("BrandNotFound", "Brand was not found.", ErrorType.NotFound));

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category is null)
                return Result<CreateProductResult>.Failure(new Error("CategoryNotFound", "Category was not found.", ErrorType.NotFound));

            var utcNow = DateTime.UtcNow;
            var attributeTuples = request.Attributes
                .Select(attribute => (attribute.Name, attribute.Value))
                .ToList();

            var product = Product.Create(
                request.Name,
                request.Description,
                request.BrandId,
                request.CategoryId,
                request.Price,
                request.ImageUrl,
                attributeTuples,
                utcNow);

            var attributeValuesJson = ProductAttributeJsonBuilder.BuildValues(attributeTuples);

            await _productRepository.AddAsync(product, brand.Name, attributeValuesJson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreateProductResult>.Success(new CreateProductResult(product.Id));
        }
    }
}
