using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Clients.Requests;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Clients
{
    internal class CustomerServiceClient : ICustomerServiceClient
    {
        private const string createEndpoint = "api/internal/customers";

        private readonly HttpClient _httpClient;

        public CustomerServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateAsync(Guid userId, string name, string email, CancellationToken ct)
        {
            var request = new CreateCustomerRequest(userId, name, email);

            using var response = await _httpClient.PostAsJsonAsync(
                createEndpoint,
                request,
                ct);

            response.EnsureSuccessStatusCode();
        }
    }
}
