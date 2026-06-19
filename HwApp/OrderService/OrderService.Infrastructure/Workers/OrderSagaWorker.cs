using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrderService.Application.Abstractions.Exceptions;
using OrderService.Application.Orders.Saga;
using OrderService.Domain.Orders;
using OrderService.Infrastructure.Options;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Workers
{
    internal sealed class OrderSagaWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly OrderSagaOptions _options;
        private readonly string _workerId;

        public OrderSagaWorker(
            IServiceProvider serviceProvider,
            IOptions<OrderSagaOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;

            _workerId =
                Environment.GetEnvironmentVariable("HOSTNAME")
                ?? Environment.MachineName;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var processedAny = await ProcessBatchAsync(stoppingToken);

                    if (!processedAny)
                    {
                        await Task.Delay(_options.PollingInterval, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch
                {
                    await Task.Delay(_options.PollingInterval, stoppingToken);
                }
            }
        }

        private async Task<bool> ProcessBatchAsync(CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var sagaProcessor = scope.ServiceProvider.GetRequiredService<IOrderSagaProcessor>();

            var now = DateTime.UtcNow;

            var orderIds = await db.Orders
                .AsNoTracking()
                .Where(order =>
                    order.Status == OrderStatus.Processing &&
                    (order.NextRetryAt == null || order.NextRetryAt <= now) &&
                    (order.LockedBy == null || order.LockedUntil == null || order.LockedUntil <= now))
                .OrderBy(order => order.UpdatedAt)
                .Select(order => order.Id)
                .Take(_options.BatchSize)
                .ToListAsync(cancellationToken);

            if (orderIds.Count == 0)
                return false;

            var processedAny = false;

            foreach (var orderId in orderIds)
            {
                var processed = await TryProcessOrderAsync(
                    db,
                    sagaProcessor,
                    orderId,
                    cancellationToken);

                processedAny = processedAny || processed;
            }

            return processedAny;
        }

        private async Task<bool> TryProcessOrderAsync(
            DatabaseContext db,
            IOrderSagaProcessor sagaProcessor,
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var lockedUntil = now.Add(_options.LockDuration);

            var lockedRows = await db.Orders
                .Where(order =>
                    order.Id == orderId &&
                    order.Status == OrderStatus.Processing &&
                    (order.NextRetryAt == null || order.NextRetryAt <= now) &&
                    (order.LockedBy == null || order.LockedUntil == null || order.LockedUntil <= now))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(order => order.LockedBy, _workerId)
                    .SetProperty(order => order.LockedUntil, lockedUntil)
                    .SetProperty(order => order.UpdatedAt, now),
                    cancellationToken);

            if (lockedRows == 0)
                return false;

            var order = await db.Orders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);

            if (order is null)
                return false;

            try
            {
                await sagaProcessor.ProcessAsync(order, cancellationToken);
            }
            catch (ExternalServiceException ex)
            {
                order.ScheduleRetry(ex.Message, CalculateDelay(order.RetryCount));
            }
            catch (Exception ex)
            {
                order.ScheduleRetry(
                    ex.Message,
                    CalculateDelay(order.RetryCount));
            }
            finally
            {
                order.ReleaseLock();
            }

            await db.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static TimeSpan CalculateDelay(int retryCount)
        {
            var seconds = retryCount switch
            {
                0 => 10,
                1 => 30,
                2 => 60,
                3 => 120,
                _ => 300
            };

            return TimeSpan.FromSeconds(seconds);
        }
    }
}