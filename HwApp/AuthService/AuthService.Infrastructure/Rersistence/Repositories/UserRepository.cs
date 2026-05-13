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

            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken ct)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login, ct);
        }

        public async Task UpdateStatusToActiveAsync(Guid userId, CancellationToken ct)
        {
            await _dbContext.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(
                    u => u.SetProperty(
                        x => x.Status, Domain.Enums.UserStatus.Active
                    ), 
                    ct
                );
        }

        public async Task UpdateStatusToBlockedAsync(Guid userId, CancellationToken ct)
        {
            await _dbContext.Users
               .Where(u => u.Id == userId)
               .ExecuteUpdateAsync(
                   u => u.SetProperty(
                       x => x.Status, Domain.Enums.UserStatus.Blocked
                   ),
                   ct
               );
        }
    }
}
