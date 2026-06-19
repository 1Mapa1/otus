using BillingService.Domain.Accounts;
using BillingService.Domain.AccountTransactions;
using BillingService.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    internal class AccountTransactionConfiguration : IEntityTypeConfiguration<AccountTransaction>
    {
        public void Configure(EntityTypeBuilder<AccountTransaction> builder)
        {
            builder.ToTable("account_transactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.OrderId)
                .HasColumnName("order_id");

            builder.Property(x => x.PaymentId)
               .HasColumnName("payment_id");

            builder.Property(x => x.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.BalanceAfter)
                .HasColumnName("balance_after")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.HeldAfter)
                .HasColumnName("held_after")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.BalanceBefore)
                .HasColumnName("balance_before")
                .HasPrecision(18, 2)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.HeldBefore)
                .HasColumnName("held_before")
                .HasPrecision(18, 2)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.PaymentId);

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Payment>()
                .WithMany()
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
