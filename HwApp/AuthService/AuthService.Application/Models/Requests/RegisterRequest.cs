namespace AuthService.Application.Models.Requests
{
    public sealed record RegisterRequest
    {
        public required string Login { get; set; }

        public required string Password { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }
    }
}
