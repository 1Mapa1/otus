using CatalogService.Application.Abstractions.Persistence;
using CatalogService.Application.Common;
using MediatR;

namespace CatalogService.Application.Categories.UpdateCategory
{
    internal sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly ICategoryWriteRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryHandler(ICategoryWriteRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Failure(new Error("ValidationError", "Name is required.", ErrorType.Validation));

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category is null)
                return Result.Failure(new Error("CategoryNotFound", "Category was not found.", ErrorType.NotFound));

            if (await _categoryRepository.ExistsByNameAsync(request.Name, request.CategoryId, cancellationToken))
                return Result.Failure(new Error("CategoryNameConflict", "Category name already exists.", ErrorType.Conflict));

            category.Rename(request.Name, DateTime.UtcNow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
