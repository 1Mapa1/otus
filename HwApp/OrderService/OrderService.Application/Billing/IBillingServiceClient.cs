namespace OrderService.Application
{
    namespace Billing
    {
        public interface IBillingServiceClient
        {
            Task<BillingWithdrawResult> WithdrawAsync(
                Guid userId,
                Guid orderId,
                decimal amount,
                CancellationToken cancellationToken = default);
        }
    }
}
