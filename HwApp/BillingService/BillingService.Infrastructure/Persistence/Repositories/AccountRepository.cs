using BillingService.Application.Accounts;
using BillingService.Domain.Accounts;
using BillingService.Domain.AccountTransactions;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infrastructure.Persistence.Repositories
{
    internal sealed class AccountRepository : IAccountRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AccountRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task EnsureAccountAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            await _databaseContext.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO accounts (
                    user_id,
                    balance,
                    held_amount,
                    created_at,
                    updated_at
                )
                VALUES (
                    {userId},
                    {0m},
                    {0m},
                    {now},
                    {now}
                )
                ON CONFLICT (user_id) DO NOTHING;
                """,
                cancellationToken);
        }

        public async Task<DepositOperationResult> DepositAsync(
            Guid userId,
            decimal amount,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            await EnsureAccountAsync(userId, cancellationToken);

            var updatedAt = DateTime.UtcNow;

            var rows = await _databaseContext.Database
                .SqlQuery<BalanceChangeSqlResult>($"""
                    UPDATE accounts
                    SET balance = balance + {amount},
                        updated_at = {updatedAt}
                    WHERE user_id = {userId}
                    RETURNING balance - {amount} AS "BalanceBefore",
                              balance AS "BalanceAfter",
                              held_amount AS "HeldAmountAfter",
                              held_amount AS "HeldAmountBefore"
                    """)
                .ToListAsync(cancellationToken);

            var result = rows.SingleOrDefault();

            if (result is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return DepositOperationResult.AccountNotFound;
            }

            var accountTransaction = AccountTransaction.Deposit(
                userId: userId,
                amount: amount,
                before: new AccountBalanceSnapshot(result.BalanceBefore, result.HeldAmountBefore),
                after: new AccountBalanceSnapshot(result.BalanceAfter, result.HeldAmountAfter));

            await _databaseContext.AccountTransactions.AddAsync(
                accountTransaction,
                cancellationToken);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return DepositOperationResult.Success;
        }

        public async Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _databaseContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
        }
    }
}
