namespace AuthService.Application.Models.Responses
{
    public sealed record LoginResponse
    {
        public required string AccessToken { get; set; }
    }
}
