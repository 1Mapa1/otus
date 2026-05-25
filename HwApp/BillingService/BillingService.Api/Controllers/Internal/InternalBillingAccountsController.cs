using BillingService.Api.Contracts;
using BillingService.Application.Accounts.CreateAccount;
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
    }
}
