using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Orders.CreateOrder;
using OrderService.Application.Orders.GetMyOrders;
using OrderService.Application.Orders.GetOrderById;
using OrderService.Api.Contracts;

namespace OrderService.Api
{
    namespace Controllers
    {
        [ApiController]
        [Route("api/orders")]
        [Authorize]
        public sealed class OrdersController : ControllerBase
        {
            private readonly ISender _sender;

            public OrdersController(ISender sender)
            {
                _sender = sender;
            }

            [HttpPost]
            public async Task<ActionResult<CreateOrderResponse>> Create(
                [FromBody] CreateOrderRequest request,
                CancellationToken cancellationToken)
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                var result = await _sender.Send(new CreateOrderCommand(userId, request.DeliverySlotId, request.Items), cancellationToken);

                return Ok(new CreateOrderResponse(result.OrderId, result.Status.ToString(), result.FailureReason.ToString()));
            }

            [HttpGet("{id:guid}")]
            public async Task<ActionResult<OrderDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                var dto = await _sender.Send(new GetOrderByIdQuery(userId, id), cancellationToken);

                if (dto is null)
                    return NotFound();

                return Ok(dto);
            }

            [HttpGet("me")]
            public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetMine(CancellationToken cancellationToken)
            {
                if (!TryGetCurrentUserId(out var userId))
                    return Unauthorized();

                var list = await _sender.Send(new GetMyOrdersQuery(userId), cancellationToken);

                return Ok(list);
            }

            private bool TryGetCurrentUserId(out Guid userId)
            {
                var idClaim = User.FindFirstValue("sub")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.Identity?.Name;

                return Guid.TryParse(idClaim, out userId);
            }
        }
    }
}
