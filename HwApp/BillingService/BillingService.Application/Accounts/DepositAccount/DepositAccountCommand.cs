using MediatR;

namespace BillingService.Application.Accounts.DepositAccount
{
    public sealed record DepositAccountCommand(
        Guid UserId,
        decimal Amount) : IRequest<DepositAccountResult>;
}