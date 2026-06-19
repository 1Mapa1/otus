using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseService.Api.Contracts;
using WarehouseService.Api.Extensions;
using WarehouseService.Application.Products.AddProductStock;
using WarehouseService.Application.Products.CreateProduct;
using WarehouseService.Application.Products.GetProducts;

namespace WarehouseService.Api.Controllers.External
{
    [ApiController]
    [Authorize]
    [Route("api/warehouse/products")]
    public sealed class WarehouseProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public WarehouseProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductsQuery(), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateProductCommand(request.Name, request.UnitPrice), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/stock")]
        public async Task<IActionResult> AddProductStock(
            Guid id,
            [FromBody] AddProductStockRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new AddProductStockCommand(id, request.Quantity), cancellationToken);

            return result.ToActionResult();
        }
    }
}
