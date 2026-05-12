using BillingService.Application.Accounts;
using BillingService.Domain.Accounts;
using BillingService.Domain.AccountTransactions;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infrastructure.Persistence.Repositories
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AccountRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task AddAsync(Account account, CancellationToken cancellationToken)
        {
            await _databaseContext.Accounts.AddAsync(account, cancellationToken);
        }

        public async Task<DepositOperationResult> DepositAsync(
            Guid userId,
            decimal amount,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var updatedAt = DateTime.UtcNow;

            var result = await _databaseContext.Database
                .SqlQuery<BalanceChangeSqlResult>($"""
                    UPDATE accounts
                    SET balance = balance + {amount},
                        updated_at = {updatedAt}
                    WHERE user_id = {userId}
                    RETURNING balance - {amount} AS "BalanceBefore",
                              balance AS "BalanceAfter"
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return DepositOperationResult.AccountNotFound;
            }

            var accountTransaction = AccountTransaction.Create(
                userId: userId,
                orderId: null,
                type: AccountTransactionType.Deposit,
                amount: amount,
                balanceBefore: result.BalanceBefore,
                balanceAfter: result.BalanceAfter);

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

        public async Task<WithdrawOperationResult> TryWithdrawAsync(
            Guid userId,
            Guid orderId,
            decimal amount,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var updatedAt = DateTime.UtcNow;

            var result = await _databaseContext.Database
                .SqlQuery<BalanceChangeSqlResult>($"""
                    UPDATE accounts
                    SET balance = balance - {amount},
                        updated_at = {updatedAt}
                    WHERE user_id = {userId}
                      AND balance >= {amount}
                    RETURNING balance + {amount} AS "BalanceBefore",
                              balance AS "BalanceAfter"
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                var accountExists = await _databaseContext.Accounts
                    .AnyAsync(a => a.UserId == userId, cancellationToken);

                await transaction.RollbackAsync(cancellationToken);

                return accountExists
                    ? WithdrawOperationResult.InsufficientFunds
                    : WithdrawOperationResult.AccountNotFound;
            }

            var accountTransaction = AccountTransaction.Create(
                userId: userId,
                orderId: orderId,
                type: AccountTransactionType.Withdraw,
                amount: amount,
                balanceBefore: result.BalanceBefore,
                balanceAfter: result.BalanceAfter);

            await _databaseContext.AccountTransactions.AddAsync(
                accountTransaction,
                cancellationToken);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return WithdrawOperationResult.Success;
        }

        private sealed class BalanceChangeSqlResult
        {
            public decimal BalanceBefore { get; init; }

            public decimal BalanceAfter { get; init; }
        }
    }
}
