namespace AuthService.Application.Interfaces
{
    public interface ICustomerServiceClient
    {
        Task CreateAsync(Guid userId, string name, string email, CancellationToken ct);
    }
}
