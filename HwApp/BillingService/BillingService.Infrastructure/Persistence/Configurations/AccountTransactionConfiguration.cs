using BillingService.Domain.Accounts;
using BillingService.Domain.AccountTransactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    internal class AccountTransactionConfiguration : IEntityTypeConfiguration<AccountTransaction>
    {
        public void Configure(EntityTypeBuilder<AccountTransaction> builder)
        {
            builder.ToTable("account_transactions");

            builder.HasKey(at => at.Id);

            builder.Property(at => at.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(at => at.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(at => at.OrderId)
                .HasColumnName("order_id");

            builder.Property(at => at.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(at => at.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(at => at.BalanceAfter)
                .HasColumnName("balance_after")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(at => at.BalanceBefore)
                .HasColumnName("balance_before")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(at => at.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.HasIndex(at => at.UserId);
            builder.HasIndex(at => at.OrderId);

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(at => at.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
