using CatalogService.Api.Extensions;
using CatalogService.Application.Products;
using CatalogService.Application.Products.ArchiveProduct;
using CatalogService.Application.Products.CreateProduct;
using CatalogService.Application.Products.RestoreProduct;
using CatalogService.Application.Products.UpdateProduct;
using CatalogService.Api.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers.External
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/catalog/products")]
    public sealed class AdminProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] UpsertProductRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateProductCommand(
                    request.Name,
                    request.Description,
                    request.BrandId,
                    request.CategoryId,
                    request.Price,
                    request.ImageUrl,
                    MapAttributes(request.Attributes)),
                cancellationToken);

            if (result.IsSuccess)
                return Accepted(new { productId = result.Value!.ProductId });

            return result.ToActionResult();
        }

        [HttpPut("{productId:guid}")]
        public async Task<IActionResult> UpdateProduct(
            Guid productId,
            [FromBody] UpsertProductRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateProductCommand(
                    productId,
                    request.Name,
                    request.Description,
                    request.BrandId,
                    request.CategoryId,
                    request.Price,
                    request.ImageUrl,
                    MapAttributes(request.Attributes)),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> ArchiveProduct(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new ArchiveProductCommand(productId), cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("{productId:guid}/restore")]
        public async Task<IActionResult> RestoreProduct(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new RestoreProductCommand(productId), cancellationToken);
            return result.ToActionResult();
        }

        private static IReadOnlyList<ProductAttributeInput> MapAttributes(IReadOnlyList<ProductAttributeRequest> attributes)
            => attributes.Select(attribute => new ProductAttributeInput(attribute.Name, attribute.Value)).ToList();
    }
}
