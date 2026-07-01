using CatalogService.Application.Common;
using CatalogService.Application.Products;

namespace CatalogService.Application.Products.Validation
{
    internal static class ProductInputValidator
    {
        public static Error? Validate(
            string? name,
            string? description,
            decimal price,
            string? imageUrl,
            IReadOnlyList<ProductAttributeInput>? attributes)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new Error("ValidationError", "Name is required.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(description))
                return new Error("ValidationError", "Description is required.", ErrorType.Validation);

            if (price <= 0)
                return new Error("ValidationError", "Price must be greater than zero.", ErrorType.Validation);

            if (string.IsNullOrWhiteSpace(imageUrl))
                return new Error("ValidationError", "ImageUrl is required.", ErrorType.Validation);

            if (!Uri.TryCreate(imageUrl.Trim(), UriKind.Absolute, out _))
                return new Error("ValidationError", "ImageUrl must be a valid absolute URI.", ErrorType.Validation);

            if (attributes is null)
                return new Error("ValidationError", "Attributes collection is required.", ErrorType.Validation);

            foreach (var attribute in attributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Name) || string.IsNullOrWhiteSpace(attribute.Value))
                    return new Error("ValidationError", "Each attribute must have name and value.", ErrorType.Validation);
            }

            return null;
        }
    }
}
