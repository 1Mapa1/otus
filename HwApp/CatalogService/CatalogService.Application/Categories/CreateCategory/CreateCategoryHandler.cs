using CatalogService.Application.Abstractions.Caching;
using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using CatalogService.Domain.Categories;
using MediatR;

namespace CatalogService.Application.Categories.CreateCategory
{
    internal sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResult>>
    {
        private readonly ICategoryWriteRepository _categoryRepository;
        private readonly ICategoryPrimaryReadRepository _categoryPrimaryReadRepository;
        private readonly ICatalogReferenceListCacheService _referenceListCacheService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryHandler(
            ICategoryWriteRepository categoryRepository,
            ICategoryPrimaryReadRepository categoryPrimaryReadRepository,
            ICatalogReferenceListCacheService referenceListCacheService,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _categoryPrimaryReadRepository = categoryPrimaryReadRepository;
            _referenceListCacheService = referenceListCacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateCategoryResult>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<CreateCategoryResult>.Failure(new Error("ValidationError", "Name is required.", ErrorType.Validation));

            if (await _categoryRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
                return Result<CreateCategoryResult>.Failure(new Error("CategoryNameConflict", "Category name already exists.", ErrorType.Conflict));

            var category = Category.Create(request.Name, DateTime.UtcNow);
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _referenceListCacheService.RefreshCategoriesAsync(
                ct => _categoryPrimaryReadRepository.GetCategoriesAsync(ct),
                cancellationToken);

            return Result<CreateCategoryResult>.Success(new CreateCategoryResult(category.Id));
        }
    }
}
