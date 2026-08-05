namespace OrderService.Domain.Orders
{
    public sealed class DeliveryAddressSnapshot
    {
        public string City { get; private set; } = null!;

        public string Street { get; private set; } = null!;

        public string House { get; private set; } = null!;

        public string? Apartment { get; private set; }

        private DeliveryAddressSnapshot()
        {
        }

        public DeliveryAddressSnapshot(string city, string street, string house, string? apartment)
        {
            City = city.Trim();
            Street = street.Trim();
            House = house.Trim();
            Apartment = string.IsNullOrWhiteSpace(apartment) ? null : apartment.Trim();
        }

        public static DeliveryAddressSnapshot Create(string city, string street, string house, string? apartment)
            => new(city, street, house, apartment);
    }
}
