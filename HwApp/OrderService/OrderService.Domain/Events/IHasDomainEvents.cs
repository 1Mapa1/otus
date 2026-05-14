namespace OrderService.Domain.Events
{
    public interface IHasDomainEvents
    {
        public IReadOnlyCollection<IDomainEvent> Events { get; }

        public void ClearEvents();
    }
}
