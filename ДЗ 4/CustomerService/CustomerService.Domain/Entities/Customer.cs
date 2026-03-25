namespace CustomerService.Domain.Entities
{
    public sealed class Customer
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string? Email { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        private Customer() { }

        public Customer(string name, string? email = null, DateTime? dateOfBirth = null)
        {
            Id = Guid.NewGuid();
            SetName(name);
            SetEmail(email);
            SetDateOfBirth(dateOfBirth);
        }

        public void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name is required.", nameof(value));

            Name = value;
        }

        public void SetEmail(string? value)
        {
            Email = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public void SetDateOfBirth(DateTime? value)
        {
            DateOfBirth = value;
        }
    }
}
