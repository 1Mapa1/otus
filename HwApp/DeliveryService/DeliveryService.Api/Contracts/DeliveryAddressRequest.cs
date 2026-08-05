namespace DeliveryService.Api.Contracts
{
    public sealed record DeliveryAddressRequest(
        string City,
        string Street,
        string House,
        string? Apartment);
}
