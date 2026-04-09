namespace AuthService.Application.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string login)
            : base($"User with login '{login}' already exists.")
        {
        }
    }
}
