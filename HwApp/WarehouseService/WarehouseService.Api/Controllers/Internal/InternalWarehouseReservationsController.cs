using MediatR;
using Microsoft.AspNetCore.Mvc;
using WarehouseService.Api.Contracts;
using WarehouseService.Api.Extensions;
using WarehouseService.Application.Reservations.CancelReservation;
using WarehouseService.Application.Reservations.CreateReservation;

namespace WarehouseService.Api.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/warehouse/reservations")]
    public sealed class InternalWarehouseReservationsController : ControllerBase
    {
        private readonly ISender _sender;

        public InternalWarehouseReservationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            [FromBody] CreateReservationRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateReservationCommand(request.OrderId, request.UserId, request.Items), cancellationToken);

            return result.ToActionResult();

        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelReservation(
            [FromBody] CancelReservationRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CancelReservationCommand(request.OrderId), cancellationToken);

            return result.ToActionResult();

        }
    }
}
