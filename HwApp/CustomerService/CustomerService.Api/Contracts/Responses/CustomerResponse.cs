namespace CustomerService.Api.Contracts.Responses
{
    public sealed class CustomerResponse
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Email { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
