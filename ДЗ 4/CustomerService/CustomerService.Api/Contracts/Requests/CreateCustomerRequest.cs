namespace CustomerService.Api.Contracts.Requests
{
    public class CreateCustomerRequest
    {
        public required string Name { get; set; }

        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
