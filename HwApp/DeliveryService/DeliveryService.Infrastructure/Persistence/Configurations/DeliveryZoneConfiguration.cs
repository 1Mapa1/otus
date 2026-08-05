using DeliveryService.Domain.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryService.Infrastructure.Persistence.Configurations
{
    internal sealed class DeliveryZoneConfiguration : IEntityTypeConfiguration<DeliveryZone>
    {
        public void Configure(EntityTypeBuilder<DeliveryZone> builder)
        {
            builder.ToTable("delivery_zone");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.City)
                .HasColumnName("city")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
        }
    }
}
