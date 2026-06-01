namespace OrderService.Api
{
    namespace Contracts
    {
        public sealed record CreateOrderResponse(
            Guid Id,
            string Status,
            string? FailureReason);
    }
}
