namespace BillingService.Domain.AccountTransactions
{
    public enum AccountTransactionType
    {
        Deposit = 1,

        [Obsolete("Legacy transaction type. Use Authorize/Capture flow for new payments.")]
        Withdraw = 2,

        Authorize = 3,
        Capture = 4,
        CancelAuthorization = 5
    }
}