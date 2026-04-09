namespace AuthService.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string Hash(string password);

        bool Verify(string passwordHash, string password);
    }
}
