namespace OrderService.Application.Abstractions.Clients.Catalog
{
    public enum CatalogClientErrorCode
    {
        ProductNotFound = 1,
        ProductInactive = 2,
        InvalidItems = 3,
        Unknown = 100
    }
}
