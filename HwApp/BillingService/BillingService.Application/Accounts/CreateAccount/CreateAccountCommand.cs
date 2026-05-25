using MediatR;

namespace BillingService.Application.Accounts.CreateAccount
{
    public sealed record CreateAccountCommand(
        Guid UserId) : IRequest;
}
