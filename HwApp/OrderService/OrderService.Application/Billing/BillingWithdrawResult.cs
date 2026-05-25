namespace OrderService.Application.Billing
{
    public sealed record BillingWithdrawResult(BillingWithdrawStatus Status)
    { 
        public static BillingWithdrawResult Success()
        {
            return new BillingWithdrawResult(BillingWithdrawStatus.Success);
        }

        public static BillingWithdrawResult InsufficientFunds()
        {
            return new BillingWithdrawResult(BillingWithdrawStatus.InsufficientFunds);
        }

        public static BillingWithdrawResult InvalidAmount()
        {
            return new BillingWithdrawResult(BillingWithdrawStatus.InvalidAmount);
        }

        public static BillingWithdrawResult AccountNotFound()
        {
            return new BillingWithdrawResult(BillingWithdrawStatus.AccountNotFound);
        }

        public static BillingWithdrawResult UnknownError()
        {
            return new BillingWithdrawResult(BillingWithdrawStatus.UnknownError);
        }
    }
}
