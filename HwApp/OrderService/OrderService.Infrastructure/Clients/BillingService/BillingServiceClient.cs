using OrderService.Application.Billing;
using OrderService.Infrastructure.Clients.BillingService.Requests;
using OrderService.Infrastructure.Clients.BillingService.Responses;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.Clients.Billing
{
    internal sealed class BillingServiceClient : IBillingServiceClient
    {
        private const string WithdrawEndpoint = "api/internal/billing/accounts/withdraw";

        private const string SuccessStatus = "Success";
        private const string InsufficientFundsStatus = "InsufficientFunds";

        private readonly HttpClient _httpClient;

        public BillingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BillingWithdrawResult> WithdrawAsync(
            Guid userId,
            Guid orderId,
            decimal amount,
            CancellationToken cancellationToken = default)
        {
            var request = new WithdrawRequest(userId, orderId, amount);

            using var response = await _httpClient.PostAsJsonAsync(
                WithdrawEndpoint,
                request,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return BillingWithdrawResult.InvalidAmount();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return BillingWithdrawResult.AccountNotFound();

            if (response.StatusCode != HttpStatusCode.OK)
                return BillingWithdrawResult.UnknownError();

            var body = await response.Content.ReadFromJsonAsync<WithdrawResponse>(
                cancellationToken: cancellationToken);

            return body?.Status switch
            {
                SuccessStatus => BillingWithdrawResult.Success(),
                InsufficientFundsStatus => BillingWithdrawResult.InsufficientFunds(),
                _ => BillingWithdrawResult.UnknownError()
            };
        }
    }
}
