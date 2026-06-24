namespace CustomerService.Domain.Entities
{
    public class CustomerAddress
    {
        public Guid Id { get; private set; }

        public Guid CustomerId { get; private set; }

        public string City { get; private set; } = string.Empty;

        public string Street { get; private set; } = string.Empty;

        public int House { get; private set; }

        public int Apartment { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private CustomerAddress() { }

        private CustomerAddress(Guid customerId, string city, string street, int house, int apartment)
        {
            var now = DateTime.UtcNow;

            Id = Guid.NewGuid();
            CustomerId = customerId;
            City = city;
            Street = street;
            House = house;
            Apartment = apartment;
            IsActive = true;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public static CustomerAddress Create(Guid customerId, string city, string street, int house, int apartment)
            => new CustomerAddress(customerId, city, street, house, apartment);

        public void Update(string city, string street, int house, int apartment)
        {
            City = city;
            Street = street;
            House = house;
            Apartment = apartment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
