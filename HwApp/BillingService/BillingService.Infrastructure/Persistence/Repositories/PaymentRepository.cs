using BillingService.Application.Payments;
using BillingService.Application.Payments.Operations;
using BillingService.Domain.AccountTransactions;
using BillingService.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BillingService.Infrastructure.Persistence.Repositories
{
    internal sealed class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseContext _databaseContext;

        public PaymentRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<AuthorizeOperationResult> AuthorizeAsync(Guid userId, Guid orderId, decimal amount, CancellationToken cancellationToken)
        {
            var existing = await _databaseContext.Payments
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

            if (existing is not null)
            {
                if (existing.Status is PaymentStatus.AuthorizationCanceled)
                    return AuthorizeOperationResult.InvalidPaymentState();

                return AuthorizeOperationResult.Success(existing.Id, existing.Amount);
            }

            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            try
            {
                var updatedAt = DateTime.UtcNow;

                var rows = await _databaseContext.Database
                    .SqlQuery<BalanceChangeSqlResult>($"""
                    UPDATE accounts
                    SET held_amount = held_amount + {amount},
                        updated_at = {updatedAt}
                    WHERE user_id = {userId}
                        AND balance - held_amount >= {amount}
                    RETURNING balance AS "BalanceBefore",
                              balance AS "BalanceAfter",
                              held_amount AS "HeldAmountAfter",
                              held_amount - {amount} AS "HeldAmountBefore"
                    """)
                    .ToListAsync(cancellationToken);

                var result = rows.SingleOrDefault();

                if (result is null)
                {
                    var accountExists = await _databaseContext.Accounts
                        .AnyAsync(a => a.UserId == userId, cancellationToken);

                    await transaction.RollbackAsync(cancellationToken);

                    return accountExists
                        ? AuthorizeOperationResult.InsufficientFunds()
                        : AuthorizeOperationResult.AccountNotFound();
                }

                var payment = Payment.Create(
                    orderId: orderId,
                    userId: userId,
                    amount: amount);

                var accountTransaction = AccountTransaction.Authorize(
                    userId: userId,
                    orderId: orderId,
                    paymentId: payment.Id,
                    amount: amount,
                    before: new AccountBalanceSnapshot(result.BalanceBefore, result.HeldAmountBefore),
                    after: new AccountBalanceSnapshot(result.BalanceAfter, result.HeldAmountAfter));

                await _databaseContext.Payments.AddAsync(
                    payment,
                    cancellationToken);

                await _databaseContext.AccountTransactions.AddAsync(
                    accountTransaction,
                    cancellationToken);

                await _databaseContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return AuthorizeOperationResult.Success(payment.Id, payment.Amount);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException
               && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                await transaction.RollbackAsync(cancellationToken);

                var payment = await _databaseContext.Payments
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

                if (payment is null)
                    throw;

                if (payment.Status is PaymentStatus.AuthorizationCanceled)
                    return AuthorizeOperationResult.InvalidPaymentState();

                return AuthorizeOperationResult.Success(payment.Id, payment.Amount);
            }
        }

        public async Task<CancelAuthorizationOperationResult> CancelAuthorizationAsync(Guid orderId, CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var updatedAt = DateTime.UtcNow;

            var paymentRows = await _databaseContext.Database
                .SqlQuery<PaymentMutationSqlResult>($"""
                    UPDATE payments
                    SET status = 'AuthorizationCanceled',
                        canceled_at = {updatedAt},
                        updated_at = {updatedAt}
                    WHERE order_id = {orderId}
                      AND status = 'Authorized'
                    RETURNING id AS "PaymentId",
                              user_id AS "UserId",
                              amount AS "Amount"
                    """)
                .ToListAsync(cancellationToken);

            var paymentRow = paymentRows.SingleOrDefault();

            if (paymentRow is null)
            {
                await transaction.RollbackAsync(cancellationToken);

                var currentPayment = await _databaseContext.Payments
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

                if (currentPayment is null)
                    return CancelAuthorizationOperationResult.PaymentNotFound;

                if (currentPayment.Status == PaymentStatus.AuthorizationCanceled)
                    return CancelAuthorizationOperationResult.Success;

                return CancelAuthorizationOperationResult.InvalidPaymentState;
            }

            var rows = await _databaseContext.Database
                .SqlQuery<BalanceChangeSqlResult>($"""
                    UPDATE accounts
                    SET held_amount = held_amount - {paymentRow.Amount},
                        updated_at = {updatedAt}
                    WHERE user_id = {paymentRow.UserId}
                        AND held_amount >= {paymentRow.Amount}
                    RETURNING balance AS "BalanceBefore",
                              balance AS "BalanceAfter",
                              held_amount + {paymentRow.Amount} AS "HeldAmountBefore",
                              held_amount AS "HeldAmountAfter"
                    """)
                .ToListAsync(cancellationToken);

            var result = rows.SingleOrDefault();

            if (result is null)
            {
                await transaction.RollbackAsync(cancellationToken);

                return CancelAuthorizationOperationResult.AccountStateConflict;
            }

            var accountTransaction = AccountTransaction.CancelAuthorization(
                userId: paymentRow.UserId,
                orderId: orderId,
                paymentId: paymentRow.PaymentId,
                amount: paymentRow.Amount,
                before: new AccountBalanceSnapshot(result.BalanceBefore, result.HeldAmountBefore),
                after: new AccountBalanceSnapshot(result.BalanceAfter, result.HeldAmountAfter));

            await _databaseContext.AccountTransactions.AddAsync(
                accountTransaction,
                cancellationToken);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return CancelAuthorizationOperationResult.Success;
        }

        public async Task<CaptureOperationResult> CaptureAsync(Guid orderId, CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var updatedAt = DateTime.UtcNow;

            var capturedPayments = await _databaseContext.Database
                .SqlQuery<PaymentMutationSqlResult>($"""
                    UPDATE payments
                    SET status = 'Captured',
                        captured_at = {updatedAt},
                        updated_at = {updatedAt}
                    WHERE order_id = {orderId}
                      AND status = 'Authorized'
                    RETURNING id AS "PaymentId",
                              user_id AS "UserId",
                              amount AS "Amount"
                    """)
                .ToListAsync(cancellationToken);

            var capturedPayment = capturedPayments.SingleOrDefault();

            if (capturedPayment is null)
            {
                await transaction.RollbackAsync(cancellationToken);

                var currentPayment = await _databaseContext.Payments
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

                if (currentPayment is null)
                    return CaptureOperationResult.PaymentNotFound();

                if (currentPayment.Status == PaymentStatus.Captured)
                    return CaptureOperationResult.Success(currentPayment.Id, currentPayment.Amount);

                return CaptureOperationResult.InvalidPaymentState();
            }

            var rows = await _databaseContext.Database
                .SqlQuery<BalanceChangeSqlResult>($"""
                    UPDATE accounts
                    SET held_amount = held_amount - {capturedPayment.Amount},
                        balance = balance - {capturedPayment.Amount},
                        updated_at = {updatedAt}
                    WHERE user_id = {capturedPayment.UserId}
                        AND held_amount >= {capturedPayment.Amount}
                        AND balance >= {capturedPayment.Amount}
                    RETURNING balance + {capturedPayment.Amount} AS "BalanceBefore",
                              balance AS "BalanceAfter",
                              held_amount + {capturedPayment.Amount} AS "HeldAmountBefore",
                              held_amount AS "HeldAmountAfter"
                    """)
                .ToListAsync(cancellationToken);

            var result = rows.SingleOrDefault();

            if (result is null)
            {
                await transaction.RollbackAsync(cancellationToken);

                return CaptureOperationResult.AccountStateConflict();
            }

            var accountTransaction = AccountTransaction.Capture(
                userId: capturedPayment.UserId,
                orderId: orderId,
                paymentId: capturedPayment.PaymentId,
                amount: capturedPayment.Amount,
                before: new AccountBalanceSnapshot(result.BalanceBefore, result.HeldAmountBefore),
                after: new AccountBalanceSnapshot(result.BalanceAfter, result.HeldAmountAfter));

            await _databaseContext.AccountTransactions.AddAsync(
                accountTransaction,
                cancellationToken);

            await _databaseContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return CaptureOperationResult.Success(capturedPayment.PaymentId, capturedPayment.Amount);
        }

        private sealed class PaymentMutationSqlResult
        {
            public Guid PaymentId { get; set; }

            public Guid UserId { get; set; }

            public decimal Amount { get; set; }
        }
    }
}
