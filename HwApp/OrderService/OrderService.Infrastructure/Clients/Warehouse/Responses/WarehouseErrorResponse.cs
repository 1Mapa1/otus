namespace OrderService.Infrastructure.Clients.Warehouse.Responses
{
    internal record WarehouseErrorResponse(
        string ErrorCode,
        string Message)
    {
        public string ErrorMessage => ErrorCode + ": " + Message;
    }
}
