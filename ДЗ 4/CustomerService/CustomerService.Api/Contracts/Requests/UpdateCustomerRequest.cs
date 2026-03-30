namespace CustomerService.Api.Contracts.Requests
{
    public class UpdateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
