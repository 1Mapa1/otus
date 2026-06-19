using DeliveryService.Api.Contracts;
using DeliveryService.Api.Extensions;
using DeliveryService.Application.Reservations.CancelReservation;
using DeliveryService.Application.Reservations.CreateReservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Api.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/delivery/reservations")]
    public sealed class InternalDeliveryReservationsController : ControllerBase
    {
        private readonly ISender _sender;

        public InternalDeliveryReservationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            [FromBody] CreateReservationRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new CreateReservationCommand(request.OrderId, request.UserId, request.DeliverySlotId), cancellationToken);

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
