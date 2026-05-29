namespace NotificationService.Domain.Notifications
{
    public sealed class Notification
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid? OrderId { get; private set; }

        public NotificationType Type { get; private set; }

        public string Subject { get; private set; } = string.Empty;

        public string Body { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; }

        private Notification() { }

        private Notification(
            Guid id,
            Guid userId,
            Guid? orderId,
            NotificationType type,
            string subject,
            string body,
            DateTime createdAt)
        {
            Id = id;
            UserId = userId;
            OrderId = orderId;
            Type = type;
            Subject = subject;
            Body = body;
            CreatedAt = createdAt;
        }

        public static Notification CreateOrderPaid(
            Guid userId,
            Guid orderId,
            decimal price)
        {
            return new Notification(
                Guid.NewGuid(),
                userId,
                orderId,
                NotificationType.OrderPaid,
                "Order paid",
                $"Your order {orderId} has been paid successfully. Amount: {price}.",
                DateTime.UtcNow);
        }

        public static Notification CreateOrderRejected(
            Guid userId,
            Guid orderId,
            decimal price,
            string failureReason,
            string? failureDetails)
        {
            return new Notification(
                Guid.NewGuid(),
                userId,
                orderId,
                NotificationType.OrderRejected,
                "Order rejected",
                $"Your order {orderId} was rejected. Amount: {price}. Reason: {failureReason}. Details: {failureDetails}.",
                DateTime.UtcNow);
        }
    }
}
