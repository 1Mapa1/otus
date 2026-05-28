namespace OrderService.Domain.Orders
{
    public class OrderItem
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid ProductId { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public decimal UnitPrice { get; private set; }

        public int Quantity { get; private set; }

        public decimal TotalPrice { get; private set; }

        private OrderItem() { }

        public static OrderItem Create(Guid orderId, Guid productId, string name, decimal unitPrice, int quantity, decimal totalPrice)
        {
            return new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = productId,
                Name = name,
                UnitPrice = unitPrice,
                Quantity = quantity,
                TotalPrice = totalPrice
            };
        }
    }
}
