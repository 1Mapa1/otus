namespace DeliveryService.Api.Contracts
{
    public sealed record GetAvailableDeliverySlotsRequest(
        DeliveryAddressRequest Address);
}
