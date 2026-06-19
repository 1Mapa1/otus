namespace OrderService.Application.Abstractions.Idempotency
{
    public interface IRequestHashCalculator
    {
        string Calculate<TRequest>(TRequest request);
    }
}
