using AuthService.Application.Models.Requests;
using AuthService.Application.Models.Responses;

namespace AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        public Task RegisterAsync(RegisterRequest request, CancellationToken ct);

        public Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    }
}
