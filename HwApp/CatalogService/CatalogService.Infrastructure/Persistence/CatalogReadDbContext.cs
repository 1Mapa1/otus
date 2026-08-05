using CatalogService.Domain.Brands;
using CatalogService.Domain.Categories;
using CatalogService.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence
{
    internal sealed class CatalogReadDbContext : DbContext
    {
        public CatalogReadDbContext(DbContextOptions<CatalogReadDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();

        public DbSet<Brand> Brands => Set<Brand>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<ProductReadModel> ProductReadModels => Set<ProductReadModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogReadDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
