using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces;
using AuthService.Application.Models.Requests;
using AuthService.Application.Models.Responses;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services
{
    internal class AuthenticationService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerServiceClient _customerClient;
        private readonly IPasswordHasherService _hasherService;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthenticationService(IUserRepository userRepository, ICustomerServiceClient customerServiceClient, IPasswordHasherService hasherService, IJwtTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _customerClient = customerServiceClient;
            _hasherService = hasherService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetByLoginAsync(request.Login, ct);

            if (user is not null && user.Status != UserStatus.Pending)
                throw new UserAlreadyExistsException(request.Login);

            if (user is null)
            {
                user = new User(request.Login, _hasherService.Hash(request.Password));

                await _userRepository.AddAsync(user, ct);
            }

            await _customerClient.CreateAsync(user.Id, request.Name, request.Email, ct);

            await _userRepository.UpdateStatusToActiveAsync(user.Id, ct);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetByLoginAsync(request.Login, ct);

            if (user is null)
                throw new InvalidCredentialsException();
            
            if (user.Status != UserStatus.Active)
                throw new UserNotActiveException(user.Id);

            var isPasswordValid = _hasherService.Verify(user.PasswordHash, request.Password);

            if (!isPasswordValid)
                throw new InvalidCredentialsException();

            return new LoginResponse { AccessToken = _tokenGenerator.GenerateToken(user) };
        }
    }
}
