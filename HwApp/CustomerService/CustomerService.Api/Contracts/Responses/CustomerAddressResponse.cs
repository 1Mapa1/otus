namespace CustomerService.Api.Contracts.Responses
{
    public sealed record CustomerAddressResponse(
        Guid Id,
        string City,
        string Street,
        int House,
        int Apartment,
        DateTime UpdatedAt);
}
