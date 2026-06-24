namespace CustomerService.Api.Contracts.Requests
{
    public sealed record UpsertCustomerAddressRequest(
        string City,
        string Street,
        int House,
        int Apartment);
}
