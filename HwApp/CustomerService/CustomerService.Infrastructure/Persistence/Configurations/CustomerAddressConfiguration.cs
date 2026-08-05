using CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerService.Infrastructure.Persistence.Configurations
{
    internal class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            builder.ToTable("customer_addresses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            builder.Property(x => x.City)
                .HasColumnName("city")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Street)
                .HasColumnName("street")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.House)
                .HasColumnName("house")
                .IsRequired();

            builder.Property(x => x.IsActive)
               .HasColumnName("is_active")
               .IsRequired();

            builder.Property(x => x.Apartment)
                .HasColumnName("apartment")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.HasIndex(x => x.CustomerId);

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
