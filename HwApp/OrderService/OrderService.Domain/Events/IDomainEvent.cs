namespace OrderService.Domain.Events
{
    public interface IDomainEvent
    {
        public string Key { get; }
    }
}
