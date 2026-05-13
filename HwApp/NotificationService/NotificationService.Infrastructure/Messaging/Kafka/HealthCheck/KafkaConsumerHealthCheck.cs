using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotificationService.Infrastructure.Messaging.Kafka.HealthCheck
{
    internal sealed class KafkaConsumerHealthCheck : IHealthCheck
    {
        private readonly KafkaConsumerState _state;

        public KafkaConsumerHealthCheck(KafkaConsumerState state)
        {
            _state = state;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (!_state.Started)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Kafka consumer is not started."));
            }

            if (!_state.Subscribed)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Kafka consumer is not subscribed to topics."));
            }

            if (_state.LastPollAt is null)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Kafka consumer is started but has not polled yet."));
            }

            var pollAge = DateTimeOffset.UtcNow - _state.LastPollAt.Value;

            if (pollAge > TimeSpan.FromSeconds(30))
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy(
                        $"Kafka consumer last poll was {pollAge.TotalSeconds:N0} seconds ago. Last error: {_state.LastError ?? "none"}"));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy("Kafka consumer is running."));
        }
    }
}
