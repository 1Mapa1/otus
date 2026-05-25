namespace CustomerService.Domain.Interfaces
{
    public interface IHasDomainEvents
    {
        public IReadOnlyCollection<IDomainEvent> Events { get; }

        public void ClearEvents();
    }
}
