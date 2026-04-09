using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Security
{
    internal class PasswordHashService : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string Hash(string password)
        {
            var hasher = new PasswordHasher<User>();

            return _hasher.HashPassword(null!, password);
        }

        public bool Verify(string passwordHash, string password)
        {
            var result = _hasher.VerifyHashedPassword(null!, passwordHash, password);

            return result == PasswordVerificationResult.Success;
        }
    }
}
