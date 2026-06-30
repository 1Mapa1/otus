namespace DeliveryService.Domain.Reservations
{
    public sealed record DeliveryAddress(string City, string Street, string House, string? Apartment)
    {
        public static DeliveryAddress Create(string city, string street, string house, string? apartment)
            => new(
                city.Trim(),
                street.Trim(),
                house.Trim(),
                string.IsNullOrWhiteSpace(apartment) ? null : apartment.Trim());

        public DeliveryAddressSnapshot ToSnapshot()
            => new(City, Street, House, Apartment);
    }
}
