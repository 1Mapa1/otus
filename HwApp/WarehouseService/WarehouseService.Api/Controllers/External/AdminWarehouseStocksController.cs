using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using WarehouseService.Api.Contracts;
using WarehouseService.Api.Extensions;
using WarehouseService.Application.Stocks.GetStockById;
using WarehouseService.Application.Stocks.GetStockMovements;
using WarehouseService.Application.Stocks.GetStocks;
using WarehouseService.Application.Stocks.StockIncome;

namespace WarehouseService.Api.Controllers.External
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/warehouse/stocks")]
    public sealed class AdminWarehouseStocksController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminWarehouseStocksController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetStocksQuery(), cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetStock(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetStockByIdQuery(productId), cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("{productId:guid}/income")]
        public async Task<IActionResult> Income(
            Guid productId,
            [FromBody] StockIncomeRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new StockIncomeCommand(productId, request.Quantity),
                cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{productId:guid}/movements")]
        public async Task<IActionResult> GetMovements(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetStockMovementsQuery(productId), cancellationToken);

            return result.ToActionResult();
        }
    }
}
