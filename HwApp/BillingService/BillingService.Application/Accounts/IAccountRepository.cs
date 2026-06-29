using BillingService.Domain.Accounts;

namespace BillingService.Application.Accounts
{
    public interface IAccountRepository
    {
        Task EnsureAccountAsync(Guid userId, CancellationToken cancellationToken);

        Task<DepositOperationResult> DepositAsync(Guid userId, decimal amount, CancellationToken cancellationToken);

        Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
