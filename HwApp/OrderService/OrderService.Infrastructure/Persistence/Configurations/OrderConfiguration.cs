using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Orders;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.DeliverySlotId)
                .HasColumnName("delivery_slot_id")
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .HasColumnName("total_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.SagaStep)
                .HasColumnName("saga_step")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.PaymentId)
                .HasColumnName("payment_id");

            builder.Property(x => x.StockReservationId)
                .HasColumnName("stock_reservation_id");

            builder.Property(x => x.DeliveryReservationId)
                .HasColumnName("delivery_reservation_id");

            builder.Property(x => x.FailureReason)
                .HasColumnName("failure_reason")
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.FailureDetails)
                .HasColumnName("failure_details")
                .HasMaxLength(256);

            builder.Property(x => x.RetryCount)
                .HasColumnName("retry_count")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.NextRetryAt)
               .HasColumnName("next_retry_at");

            builder.Property(x => x.LockedBy)
                .HasColumnName("locked_by")
                .HasMaxLength(256);

            builder.Property(x => x.LockedUntil)
               .HasColumnName("locked_until");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.CompletedAt)
               .HasColumnName("completed_at");

            builder.Property(x => x.RejectedAt)
               .HasColumnName("rejected_at");

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.DeliverySlotId);

            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(x => x.Events);
        }
    }
}
