using OrderService.Infrastructure.Clients.Warehouse.Dto;

namespace OrderService.Infrastructure.Clients.Warehouse.Responses
{
    internal sealed record CreateReservationErrorResponses(
        string ErrorCode,
        string? Message,
        IReadOnlyList<ProductUnavailableDto>? UnavailableItems) : WarehouseErrorResponse(ErrorCode, Message ?? string.Empty);
}
