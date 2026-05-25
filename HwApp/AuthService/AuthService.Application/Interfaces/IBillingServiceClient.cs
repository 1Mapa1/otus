namespace AuthService.Application.Interfaces
{
    public interface IBillingServiceClient
    {
        Task CreateAccountAsync(Guid userId, CancellationToken ct);
    }
}
