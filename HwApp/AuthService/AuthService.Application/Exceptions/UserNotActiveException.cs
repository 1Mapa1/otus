namespace AuthService.Application.Exceptions
{
    public class UserNotActiveException : Exception
    {
        public UserNotActiveException(Guid userId)
            : base($"User '{userId}' is not active.")
        {
        }
    }
}
