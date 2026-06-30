using DeliveryService.Application.Zones;
using DeliveryService.Domain.Zones;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Infrastructure.Persistence.Repositories
{
    internal sealed class DeliveryZoneRepository : IDeliveryZoneRepository
    {
        private readonly DatabaseContext _databaseContext;

        public DeliveryZoneRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task AddAsync(DeliveryZone zone, CancellationToken cancellationToken)
        {
            await _databaseContext.DeliveryZones.AddAsync(zone, cancellationToken);
        }

        public async Task<DeliveryZone?> GetByIdAsync(Guid zoneId, CancellationToken cancellationToken)
        {
            return await _databaseContext.DeliveryZones
                .FirstOrDefaultAsync(zone => zone.Id == zoneId, cancellationToken);
        }

        public async Task<DeliveryZone?> GetActiveByCityAsync(string city, CancellationToken cancellationToken)
        {
            var normalizedCity = city.Trim();

            return await _databaseContext.DeliveryZones
                .AsNoTracking()
                .Where(zone => zone.IsActive &&
                               EF.Functions.ILike(zone.City, normalizedCity))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCityAsync(string city, CancellationToken cancellationToken)
        {
            var normalizedCity = city.Trim();

            return await _databaseContext.DeliveryZones
                .AnyAsync(zone => EF.Functions.ILike(zone.City, normalizedCity), cancellationToken);
        }

        public async Task<IReadOnlyList<DeliveryZone>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.DeliveryZones
                .AsNoTracking()
                .OrderBy(zone => zone.City)
                .ToListAsync(cancellationToken);
        }
    }
}
