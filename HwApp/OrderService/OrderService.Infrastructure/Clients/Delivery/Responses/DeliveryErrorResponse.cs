namespace OrderService.Infrastructure.Clients.Delivery.Responses
{
    internal record DeliveryErrorResponse(
        string ErrorCode,
        string Message)
    {
        public string ErrorMessage => ErrorCode + ": " + Message;
    }
}
