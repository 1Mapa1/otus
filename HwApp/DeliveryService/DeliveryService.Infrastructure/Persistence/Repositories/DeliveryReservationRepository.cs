using DeliveryService.Application.Reservations;
using DeliveryService.Application.Reservations.Operations;
using DeliveryService.Domain.Reservations;using DeliveryService.Domain.Slots;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DeliveryService.Infrastructure.Persistence.Repositories
{
    internal sealed class DeliveryReservationRepository : IDeliveryReservationRepository
    {
        private readonly DatabaseContext _databaseContext;

        public DeliveryReservationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<CancelReservationOperationResult> CancelAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            var reservation = await _databaseContext.DeliveryReservations
                .FirstOrDefaultAsync(r => r.OrderId == orderId, cancellationToken);

            if (reservation is null)
            {
                await transaction.CommitAsync(cancellationToken);
                return CancelReservationOperationResult.ReservationNotFound;
            }

            if (reservation.Status == DeliveryReservationStatus.Canceled)
            {
                await transaction.CommitAsync(cancellationToken);
                return CancelReservationOperationResult.Success;
            }

            var utcNow = DateTime.UtcNow;

            var reservationUpdateResult = await _databaseContext.DeliveryReservations
                .Where(r => r.Id == reservation.Id &&
                            r.Status == DeliveryReservationStatus.Reserved)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(r => r.Status, DeliveryReservationStatus.Canceled)
                    .SetProperty(r => r.CanceledAt, utcNow),
                    cancellationToken);

            if (reservationUpdateResult == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return CancelReservationOperationResult.Success;
            }

            var slotUpdateResult = await _databaseContext.DeliverySlots
                .Where(slot =>
                    slot.Id == reservation.DeliverySlotId &&
                    slot.ReservedCount > 0)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(slot => slot.ReservedCount, slot => slot.ReservedCount - 1)
                    .SetProperty(slot => slot.UpdatedAt, utcNow),
                    cancellationToken);

            if (slotUpdateResult == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return CancelReservationOperationResult.SlotStateConflict;
            }

            await transaction.CommitAsync(cancellationToken);

            return CancelReservationOperationResult.Success;
        }

        public async Task<ReserveDeliverySlotOperationResult> ReserveAsync(
            Guid orderId,
            Guid customerId,
            Guid deliverySlotId,
            DeliveryAddress address,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _databaseContext.Database
                .BeginTransactionAsync(cancellationToken);

            try
            {
                var existingReservation = await _databaseContext.DeliveryReservations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.OrderId == orderId, cancellationToken);

                if (existingReservation is not null)
                {
                    await transaction.CommitAsync(cancellationToken);

                    if (existingReservation.Status == DeliveryReservationStatus.Canceled)
                        return ReserveDeliverySlotOperationResult.InvalidReservationState();

                    return ReserveDeliverySlotOperationResult.Success(existingReservation.Id);
                }

                var normalizedCity = address.City.Trim();
                var utcNow = DateTime.UtcNow;

                var deliverySlotUpdateResult = await _databaseContext.DeliverySlots
                    .Where(slot =>
                        slot.Id == deliverySlotId &&
                        slot.Status == DeliverySlotStatus.Open &&
                        slot.TimeFrom > utcNow &&
                        slot.ReservedCount < slot.Capacity &&
                        slot.Zone.IsActive &&
                        EF.Functions.ILike(slot.Zone.City, normalizedCity))
                    .ExecuteUpdateAsync(updates => updates
                        .SetProperty(slot => slot.ReservedCount, slot => slot.ReservedCount + 1)
                        .SetProperty(slot => slot.UpdatedAt, utcNow),
                        cancellationToken);

                if (deliverySlotUpdateResult == 0)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ReserveDeliverySlotOperationResult.SlotNotAvailable();
                }

                var slot = await _databaseContext.DeliverySlots
                    .AsNoTracking()
                    .Where(s => s.Id == deliverySlotId)
                    .Select(s => new { s.ZoneId })
                    .SingleAsync(cancellationToken);

                var reservation = DeliveryReservation.Create(
                    orderId,
                    customerId,
                    deliverySlotId,
                    slot.ZoneId,
                    address.ToSnapshot());

                await _databaseContext.DeliveryReservations.AddAsync(reservation, cancellationToken);

                await _databaseContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ReserveDeliverySlotOperationResult.Success(reservation.Id);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException
                                               && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                await transaction.RollbackAsync(cancellationToken);

                _databaseContext.ChangeTracker.Clear();

                var reservation = await _databaseContext.DeliveryReservations
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

                if (reservation is null)
                    throw;

                if (reservation.Status == DeliveryReservationStatus.Canceled)
                    return ReserveDeliverySlotOperationResult.InvalidReservationState();

                return ReserveDeliverySlotOperationResult.Success(reservation.Id);
            }
        }
    }
}
