namespace DeliveryService.Domain.Reservations
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

        public DeliveryAddressSnapshot(
            string city,
            string street,
            string house,
            string? apartment)
        {
            var address = DeliveryAddress.Create(city, street, house, apartment);
            City = address.City;
            Street = address.Street;
            House = address.House;
            Apartment = address.Apartment;
        }

        public DeliveryAddressSnapshot(DeliveryAddress address)
        {
            City = address.City;
            Street = address.Street;
            House = address.House;
            Apartment = address.Apartment;
        }
    }
}
