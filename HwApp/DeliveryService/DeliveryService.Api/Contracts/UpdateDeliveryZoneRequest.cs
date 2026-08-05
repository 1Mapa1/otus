namespace DeliveryService.Api.Contracts
{
    public sealed record UpdateDeliveryZoneRequest(
        string Name,
        bool IsActive);
}
