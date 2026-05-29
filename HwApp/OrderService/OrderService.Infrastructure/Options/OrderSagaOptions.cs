namespace OrderService.Infrastructure.Options
{
    internal sealed class OrderSagaOptions
    {
        public const string SectionName = "OrderSaga";
    
        public int BatchSize { get; set; } = 10;

        public TimeSpan LockDuration { get; set; } = TimeSpan.FromSeconds(60);

        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
    }
}
