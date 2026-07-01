namespace WarehouseService.Domain.Events
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> Events { get; }

        void ClearEvents();
    }
}
