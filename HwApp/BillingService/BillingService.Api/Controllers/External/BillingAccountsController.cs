using BillingService.Api.Contracts;
using BillingService.Application.Accounts.DepositAccount;
using BillingService.Application.Accounts.GetMyAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Api.Controllers.External
{
    [ApiController]
    [Authorize]
    [Route("api/billing/accounts")]
    public sealed class BillingAccountsController : ControllerBase
    {
        private readonly ISender _sender;

        public BillingAccountsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("me")]
        public async Task<ActionResult<GetMyAccountResponse>> GetMyAccount(
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var account = await _sender.Send(
                new GetMyAccountQuery(userId),
                cancellationToken);

            if (account is null)
                return NotFound(new
                {
                    error = "AccountNotFound",
                    message = "Billing account was not found."
                });

            return Ok(new GetMyAccountResponse(
                account.UserId,
                account.Balance));
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(
            [FromBody] DepositAccountRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var result = await _sender.Send(
                new DepositAccountCommand(userId, request.Amount),
                cancellationToken);

            return result.Status switch
            {
                DepositAccountStatus.Success =>
                    NoContent(),

                DepositAccountStatus.InvalidAmount =>
                    BadRequest(new
                    {
                        error = "InvalidAmount",
                        message = "Amount must be positive."
                    }),

                DepositAccountStatus.AccountNotFound =>
                    NotFound(new
                    {
                        error = "AccountNotFound",
                        message = "Billing account was not found."
                    }),

                _ =>
                    StatusCode(StatusCodes.Status500InternalServerError)
            };
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
