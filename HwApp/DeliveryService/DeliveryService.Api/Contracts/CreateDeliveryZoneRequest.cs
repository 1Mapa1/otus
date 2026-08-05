namespace DeliveryService.Api.Contracts
{
    public sealed record CreateDeliveryZoneRequest(
        string Name,
        string City);
}
