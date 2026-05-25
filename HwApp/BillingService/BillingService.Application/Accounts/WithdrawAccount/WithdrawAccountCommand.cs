using MediatR;

namespace BillingService.Application.Accounts.WithdrawAccount
{
    public sealed record WithdrawAccountCommand(
        Guid UserId,
        Guid OrderId,
        decimal Amount) : IRequest<WithdrawAccountResult>;
}
