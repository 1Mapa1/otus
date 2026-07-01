namespace OrderService.Domain.IdempotencyRecords
{
    public enum IdempotencyRecordStatus
    {
        Processing = 0,
        Completed = 1,
    }
}