using OrderService.Infrastructure.Clients.Warehouse.Dto;

namespace OrderService.Infrastructure.Clients.Warehouse.Requests
{
    internal sealed record CreateReservationRequest(
        Guid OrderId,
        Guid UserId,
        IEnumerable<ProductQuantityDto> Items);
}
