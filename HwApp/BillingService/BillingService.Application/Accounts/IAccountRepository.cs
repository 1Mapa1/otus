using BillingService.Domain.Accounts;

namespace BillingService.Application.Accounts
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account, CancellationToken cancellationToken);

        Task<DepositOperationResult> DepositAsync(Guid userId, decimal amount, CancellationToken cancellationToken);

        Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
