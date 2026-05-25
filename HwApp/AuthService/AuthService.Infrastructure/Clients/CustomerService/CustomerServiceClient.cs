using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Clients.CustomerService.Requests;
using System.Net.Http.Json;

namespace AuthService.Infrastructure.Clients.CustomerService
{
    internal class CustomerServiceClient : ICustomerServiceClient
    {
        private const string CreateEndpoint = "api/internal/customers";

        private readonly HttpClient _httpClient;

        public CustomerServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateAsync(Guid userId, string name, string email, CancellationToken ct)
        {
            var request = new CreateCustomerRequest(userId, name, email);

            using var response = await _httpClient.PostAsJsonAsync(
                CreateEndpoint,
                request,
                ct);

            response.EnsureSuccessStatusCode();
        }
    }
}
