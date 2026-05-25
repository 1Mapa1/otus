using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Clients.BillingService.Requests;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Clients.BillingService
{
    internal sealed class BillingServiceClient : IBillingServiceClient
    {
        private const string CreateAccountEndpoint = "api/internal/billing/accounts";

        private readonly HttpClient _httpClient;

        public BillingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateAccountAsync(Guid userId, CancellationToken ct)
        {
            var request = new CreateBillingAccountRequest(userId);

            using var response = await _httpClient.PostAsJsonAsync(
                CreateAccountEndpoint,
                request,
                ct);

            response.EnsureSuccessStatusCode();
        }
    }
}
