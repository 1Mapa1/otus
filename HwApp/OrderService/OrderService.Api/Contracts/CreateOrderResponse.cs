namespace OrderService.Api
{
    namespace Contracts
    {
        public sealed record CreateOrderResponse(
            Guid OrderId,
            string Status,
            string? FailureReason);
    }
}
