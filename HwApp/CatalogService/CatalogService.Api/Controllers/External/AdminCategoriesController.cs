using CatalogService.Api.Contracts;
using CatalogService.Api.Extensions;
using CatalogService.Application.Categories.CreateCategory;
using CatalogService.Application.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers.External
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/catalog/categories")]
    public sealed class AdminCategoriesController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminCategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] UpsertCategoryRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateCategoryCommand(request.Name), cancellationToken);

            if (result.IsSuccess)
                return Accepted(new { categoryId = result.Value!.CategoryId });

            return result.ToActionResult();
        }

        [HttpPut("{categoryId:guid}")]
        public async Task<IActionResult> UpdateCategory(
            Guid categoryId,
            [FromBody] UpsertCategoryRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UpdateCategoryCommand(categoryId, request.Name), cancellationToken);
            return result.ToActionResult();
        }
    }
}
