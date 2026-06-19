using BillingService.Api.Contracts;
using BillingService.Application.Payments.AuthorizePayment;
using BillingService.Application.Payments.CancelAuthorizationPayment;
using BillingService.Application.Payments.CapturePayment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Api.Controllers.Internal
{
    [Route("api/internal/billing/payments")]
    [ApiController]
    public class InternalBillingPaymentsController : ControllerBase
    {
        private readonly ISender _sender;

        public InternalBillingPaymentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> Authorize([FromBody] AuthorizePaymentRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(new AuthorizePaymentCommand(request.UserId, request.OrderId, request.Amount), ct);

            switch (result.Status)
            {
                case AuthorizePaymentStatus.Success:
                    return Ok(new
                    {
                        paymentId = result.PaymentId,
                        authorizedAmount = result.AuthorizedAmount
                    });

                case AuthorizePaymentStatus.AccountNotFound:
                    return NotFound(new { errorCode = "AccountNotFound" });

                case AuthorizePaymentStatus.InsufficientFunds:
                    return Conflict(new { errorCode = "InsufficientFunds" });

                case AuthorizePaymentStatus.InvalidPaymentState:
                    return Conflict(new { errorCode = "InvalidPaymentState" });

                case AuthorizePaymentStatus.InvalidAmount:
                    return BadRequest(new { errorCode = "InvalidAmount" });

                default:
                    throw new InvalidOperationException(
                        $"Unknown authorize operation status: {result.Status}");
            }
        }

        [HttpPost("capture")]
        public async Task<IActionResult> Capture([FromBody] CapturePaymentRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(new CapturePaymentCommand(request.OrderId), ct);

            switch (result.Status)
            {
                case CapturePaymentStatus.Success:
                    return Ok(new
                    {
                        paymentId = result.PaymentId,
                        authorizedAmount = result.CapturedAmount
                    });

                case CapturePaymentStatus.PaymentNotFound:
                    return NotFound(new { errorCode = "PaymentNotFound" });

                case CapturePaymentStatus.InvalidPaymentState:
                    return Conflict(new { errorCode = "InvalidPaymentState" });

                case CapturePaymentStatus.AccountStateConflict:
                    return Conflict(new { errorCode = "AccountStateConflict" });

                default:
                    throw new InvalidOperationException(
                        $"Unknown capture operation status: {result.Status}");
            }
        }

        [HttpPost("cancel-authorization")]
        public async Task<IActionResult> CancelAuthorization([FromBody] CancelAuthorizationPaymentRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(new CancelAuthorizationPaymentCommand(request.OrderId), ct);

            switch (result.Status)
            {
                case CancelAuthorizationPaymentStatus.Success:
                    return Ok();

                case CancelAuthorizationPaymentStatus.InvalidPaymentState:
                    return Conflict(new { errorCode = "InvalidPaymentState" });

                case CancelAuthorizationPaymentStatus.AccountStateConflict:
                    return Conflict(new { errorCode = "AccountStateConflict" });

                default:
                    throw new InvalidOperationException(
                        $"Unknown cancel authorization operation status: {result.Status}");
            }
        }
    }
}
