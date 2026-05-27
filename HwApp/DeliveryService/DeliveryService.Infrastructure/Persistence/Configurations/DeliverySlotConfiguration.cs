using DeliveryService.Domain.Slots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryService.Infrastructure.Persistence.Configurations
{
    internal class DeliverySlotConfiguration : IEntityTypeConfiguration<DeliverySlot>
    {
        public void Configure(EntityTypeBuilder<DeliverySlot> builder)
        {
            builder.ToTable("delivery_slot");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.TimeFrom)
                .HasColumnName("time_from")
                .IsRequired();

            builder.Property(x => x.TimeTo)
                .HasColumnName("time_to")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
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
