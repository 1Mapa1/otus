using DeliveryService.Domain.Zones;

namespace DeliveryService.Application.Zones
{
    public interface IDeliveryZoneRepository
    {
        Task AddAsync(DeliveryZone zone, CancellationToken cancellationToken);

        Task<DeliveryZone?> GetByIdAsync(Guid zoneId, CancellationToken cancellationToken);

        Task<DeliveryZone?> GetActiveByCityAsync(string city, CancellationToken cancellationToken);

        Task<bool> ExistsByCityAsync(string city, CancellationToken cancellationToken);

        Task<IReadOnlyList<DeliveryZone>> GetAllAsync(CancellationToken cancellationToken);
    }
}
