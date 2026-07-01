using CatalogService.Api.Contracts.Internal;
using CatalogService.Api.Extensions;
using CatalogService.Application.Products.GetProductSnapshot;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/catalog/products")]
    public sealed class InternalCatalogProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public InternalCatalogProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("snapshot")]
        public async Task<IActionResult> GetProductSnapshot(
            [FromBody] GetProductSnapshotRequest request,
            CancellationToken cancellationToken)
        {
            var items = request.Items
                .Select(item => new GetProductSnapshotItem(item.ProductId, item.Quantity, item.ExpectedUnitPrice))
                .ToList();

            var result = await _sender.Send(new GetProductSnapshotCommand(items), cancellationToken);

            if (!result.IsSuccess)
                return result.ToActionResult();

            var value = result.Value!;
            return Ok(new
            {
                items = value.Items.Select(item => new
                {
                    productId = item.ProductId,
                    name = item.Name,
                    unitPrice = item.UnitPrice,
                    quantity = item.Quantity,
                    totalPrice = item.TotalPrice
                }),
                totalAmount = value.TotalAmount
            });
        }
    }
}
