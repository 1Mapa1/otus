using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Customers;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    internal sealed class NotificationCustomerConfiguration : IEntityTypeConfiguration<NotificationCustomer>
    {
        public void Configure(EntityTypeBuilder<NotificationCustomer> builder)
        {
            builder.ToTable("notification_customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();
        }
    }
}
