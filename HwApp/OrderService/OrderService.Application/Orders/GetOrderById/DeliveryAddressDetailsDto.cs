namespace OrderService.Application.Orders.GetOrderById
{
    public sealed record DeliveryAddressDetailsDto(
        string City,
        string Street,
        string House,
        string? Apartment);
}
