namespace NotificationService.Infrastructure.Messaging.Kafka.HealthCheck
{
    internal sealed class KafkaConsumerState
    {
        private volatile bool _started;
        private volatile bool _subscribed;
        private DateTimeOffset? _lastPollAt;
        private DateTimeOffset? _lastMessageAt;
        private string? _lastError;

        public bool Started => _started;

        public bool Subscribed => _subscribed;

        public DateTimeOffset? LastPollAt => _lastPollAt;

        public string? LastError => _lastError;

        public void MarkStarted()
        {
            _started = true;
            _lastError = null;
        }

        public void MarkSubscribed()
        {
            _subscribed = true;
            _lastError = null;
        }

        public void MarkPoll()
        {
            _lastPollAt = DateTimeOffset.UtcNow;
            _lastError = null;
        }

        public void MarkError(Exception exception)
        {
            _lastError = exception.Message;
        }

        public void MarkError(string error)
        {
            _lastError = error;
        }
    }
}
