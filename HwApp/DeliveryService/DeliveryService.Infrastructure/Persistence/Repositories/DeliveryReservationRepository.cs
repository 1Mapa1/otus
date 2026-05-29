using DeliveryService.Application.Reservations;
using DeliveryService.Application.Reservations.Operations;
using DeliveryService.Domain.Reservations;
using DeliveryService.Domain.Slots;
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

        public async Task<CancelReservationOperationResult> CancelAsync(Guid orderId, CancellationToken cancellationToken)
        {
            using var transaction = await _databaseContext.Database.BeginTransactionAsync(cancellationToken);

            var reservation = await _databaseContext.DeliveryReservations.FirstOrDefaultAsync(r => r.OrderId == orderId, cancellationToken);

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

            var reservationUpdateResult =
                await _databaseContext.DeliveryReservations
                .Where(r => r.Id == reservation.Id && 
                            r.Status == DeliveryReservationStatus.Reserved)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(r => r.Status, DeliveryReservationStatus.Canceled)
                    .SetProperty(r => r.CanceledAt, DateTime.UtcNow), cancellationToken);

            if (reservationUpdateResult == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return CancelReservationOperationResult.Success;
            }

            await _databaseContext.DeliverySlots
                .Where(ds => ds.Id == reservation.DeliverySlotId &&
                             ds.Status == DeliverySlotStatus.Reserved)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(ds => ds.Status, DeliverySlotStatus.Available)
                    .SetProperty(ds => ds.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return CancelReservationOperationResult.Success;
        }

        public async Task<ReserveDeliverySlotOperationResult> ReserveAsync(Guid orderId, Guid userId, Guid deliverySlotId, CancellationToken cancellationToken)
        {
            using var transaction = await _databaseContext.Database.BeginTransactionAsync(cancellationToken);

            try
            { 
                var existingReservation = await _databaseContext.DeliveryReservations.FirstOrDefaultAsync(r => r.OrderId == orderId, cancellationToken);

                if (existingReservation is not null)
                {
                    await transaction.CommitAsync(cancellationToken);

                    if (existingReservation.Status == DeliveryReservationStatus.Canceled)
                        return ReserveDeliverySlotOperationResult.InvalidReservationState();

                    return ReserveDeliverySlotOperationResult.Success(existingReservation.Id);
                }

                var deliverySlotUpdateResult =
                    await _databaseContext.DeliverySlots
                    .Where(ds => ds.Id == deliverySlotId &&
                                 ds.Status == DeliverySlotStatus.Available)
                    .ExecuteUpdateAsync(updates => updates
                        .SetProperty(ds => ds.Status, DeliverySlotStatus.Reserved)
                        .SetProperty(ds => ds.UpdatedAt, DateTime.UtcNow), cancellationToken);

                if (deliverySlotUpdateResult == 0)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return ReserveDeliverySlotOperationResult.SlotNotAvailable();
                }

                var reservation = DeliveryReservation.Create(orderId, userId, deliverySlotId);

                await _databaseContext.DeliveryReservations.AddAsync(reservation, cancellationToken);

                await _databaseContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ReserveDeliverySlotOperationResult.Success(reservation.Id);
            }
            catch (DbUpdateException ex) when(ex.InnerException is PostgresException postgresException
               && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                await transaction.RollbackAsync(cancellationToken);

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
