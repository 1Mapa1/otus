using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetByLoginAsync(string login, CancellationToken ct);

        public Task UpdateStatusToActiveAsync(Guid userId, CancellationToken ct);

        public Task UpdateStatusToBlockedAsync(Guid userId, CancellationToken ct);

        public Task AddAsync(User user, CancellationToken ct);
    }
}
