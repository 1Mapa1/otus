using BillingService.Domain.Accounts;
using BillingService.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.OrderId)
                .HasColumnName("order_id")
                .IsRequired();

            builder.Property(x=> x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x=> x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x=> x.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x=> x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x=> x.AuthorizedAt)
                .HasColumnName("authorized_at");

            builder.Property(x=> x.CapturedAt)
                .HasColumnName("captured_at");

            builder.Property(x=> x.CanceledAt)
                .HasColumnName("canceled_at");

            builder.HasIndex(x=> x.UserId);
            builder.HasIndex(x=> x.OrderId).IsUnique();

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x=> x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
