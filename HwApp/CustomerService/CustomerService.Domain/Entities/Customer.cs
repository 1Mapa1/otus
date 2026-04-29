using CustomerService.Domain.Events;
using CustomerService.Domain.Interfaces;

namespace CustomerService.Domain.Entities
{
    public sealed class Customer : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _events = [];

        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public DateTime? DateOfBirth { get; private set; }

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        private Customer() { }

        private Customer(Guid id, string name, string email, DateTime? dateOfBirth = null)
        {
            Id = id;
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;
        }

        public static Customer Create(Guid id, string name, string email, DateTime? dateOfBirth = null)
        {
            var c = new Customer(id, name, email, dateOfBirth);

            c.AddEvent(new CustomerCreatedEvent(id, name, email));

            return c;
        }

        public void Update(string name, string email, DateTime? dateOfBirth = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));
            Name = name;

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            Email = email;

            DateOfBirth = dateOfBirth;

            AddEvent(new CustomerUpdatedEvent(Id, name, email));
        }

        public void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name is required.", nameof(value));

            Name = value;
        }

        public void AddEvent(IDomainEvent domainEvent)
            => _events.Add(domainEvent);

        public void ClearEvents()
            => _events.Clear();
    }
}
