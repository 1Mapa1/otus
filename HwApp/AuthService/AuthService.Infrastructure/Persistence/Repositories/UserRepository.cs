using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Rersistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Rersistence.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _dbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            _dbContext = authDbContext;
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _dbContext.Users.AddAsync(user, ct);
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken ct)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login, ct);
        }
    }
}
