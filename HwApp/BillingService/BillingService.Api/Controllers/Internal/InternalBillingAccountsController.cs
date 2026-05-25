using BillingService.Api.Contracts;
using BillingService.Application.Accounts.CreateAccount;
using BillingService.Application.Accounts.WithdrawAccount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Api.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/billing/accounts")]
    public class InternalBillingAccountsController : ControllerBase
    {
        private readonly ISender _sender;

        public InternalBillingAccountsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request, CancellationToken ct)
        {
            await _sender.Send(new CreateAccountCommand(request.UserId), ct);

            return Created();
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(
                new WithdrawAccountCommand(
                    request.UserId,
                    request.OrderId,
                    request.Amount),
                ct);


            return result.Status switch
            {
                WithdrawAccountStatus.Success =>
                    Ok(new WithdrawAccountResponse("Success")),

                WithdrawAccountStatus.InsufficientFunds =>
                    Ok(new WithdrawAccountResponse("InsufficientFunds")),

                WithdrawAccountStatus.InvalidAmount =>
                    BadRequest(new
                    {
                        error = "InvalidAmount",
                        message = "Amount must be positive."
                    }),

                WithdrawAccountStatus.AccountNotFound =>
                    NotFound(new
                    {
                        error = "AccountNotFound",
                        message = "Billing account was not found."
                    }),

                _ =>
                    StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
