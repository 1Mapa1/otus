using CatalogService.Application.Products;
using CatalogService.Domain.Brands;
using CatalogService.Domain.Categories;
using CatalogService.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence
{
    internal static class CatalogDataSeeder
    {
        public static async Task SeedAsync(CatalogWriteDbContext db, CancellationToken cancellationToken = default)
        {
            if (await db.Brands.AnyAsync(cancellationToken))
                return;

            var utcNow = DateTime.UtcNow;

            var asus = Brand.Create("ASUS", utcNow);
            var lenovo = Brand.Create("Lenovo", utcNow);
            var laptops = Category.Create("Laptops", utcNow);
            var accessories = Category.Create("Accessories", utcNow);

            await db.Brands.AddRangeAsync([asus, lenovo], cancellationToken);
            await db.Categories.AddRangeAsync([laptops, accessories], cancellationToken);

            var laptopAttributes = new List<(string Name, string Value)>
            {
                ("Оперативная память", "16 GB"),
                ("Процессор", "AMD Ryzen 7"),
                ("Диагональ", "15.6\"")
            };

            var laptop = Product.Create(
                "ASUS TUF Gaming A15",
                "Игровой ноутбук для повседневных задач и игр.",
                asus.Id,
                laptops.Id,
                119999m,
                "https://cdn.example.com/products/asus-tuf-a15.jpg",
                laptopAttributes,
                utcNow);

            var mouseAttributes = new List<(string Name, string Value)>
            {
                ("Тип подключения", "USB"),
                ("DPI", "6400")
            };

            var mouse = Product.Create(
                "Lenovo Gaming Mouse",
                "Эргономичная игровая мышь.",
                lenovo.Id,
                accessories.Id,
                2999m,
                "https://cdn.example.com/products/lenovo-mouse.jpg",
                mouseAttributes,
                utcNow);

            await db.Products.AddRangeAsync([laptop, mouse], cancellationToken);

            await db.ProductReadModels.AddRangeAsync(
            [
                ProductReadModel.CreateForProduct(
                    laptop,
                    asus.Name,
                    ProductAttributeJsonBuilder.BuildValues(laptopAttributes),
                    utcNow),
                ProductReadModel.CreateForProduct(
                    mouse,
                    lenovo.Name,
                    ProductAttributeJsonBuilder.BuildValues(mouseAttributes),
                    utcNow)
            ],
                cancellationToken);

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
