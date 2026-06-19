using OrderService.Application.Abstractions.Idempotency;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OrderService.Infrastructure.Idempotency
{
    internal sealed class RequestHashCalculator : IRequestHashCalculator
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        public string Calculate<TRequest>(TRequest request)
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);
            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
