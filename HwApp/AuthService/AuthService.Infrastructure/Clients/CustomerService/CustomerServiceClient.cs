using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Clients.CustomerService.Requests;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Clients.CustomerService
{
    internal class CustomerServiceClient : ICustomerServiceClient
    {
        private const string CreateEndpoint = "api/internal/customers";

        private readonly HttpClient _httpClient;
        private readonly Microsoft.Extensions.Logging.ILogger<CustomerServiceClient> _logger;

        public CustomerServiceClient(
            HttpClient httpClient,
            Microsoft.Extensions.Logging.ILogger<CustomerServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task CreateAsync(Guid userId, string name, string email, CancellationToken ct)
        {
            var request = new CreateCustomerRequest(userId, name, email);
            var requestBody = JsonSerializer.Serialize(request);

            _logger.LogInformation(
                "Auth sends request to Customer: POST {RequestPath}. RequestBody: {RequestBody}",
                CreateEndpoint,
                requestBody);

            using var response = await _httpClient.PostAsJsonAsync(
                CreateEndpoint,
                request,
                ct);

            var responseBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogInformation(
                "Customer responded to Auth with {StatusCode}. ResponseBody: {ResponseBody}",
                (int)response.StatusCode,
                responseBody);

            response.EnsureSuccessStatusCode();
        }
    }
}
