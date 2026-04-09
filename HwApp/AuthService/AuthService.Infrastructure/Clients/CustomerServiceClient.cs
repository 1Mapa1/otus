using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Clients.Requests;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Clients
{
    internal class CustomerServiceClient : ICustomerServiceClient
    {
        private const string createEndpoint = "api/internal/profiles";

        private readonly HttpClient _httpClient;

        public CustomerServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateAsync(Guid userId, string name, CancellationToken ct)
        {
            var request = new CreateCustomerRequest(userId, name);

            using var response = await _httpClient.PostAsJsonAsync(
                createEndpoint,
                request,
                ct);

            response.EnsureSuccessStatusCode();
        }
    }
}
