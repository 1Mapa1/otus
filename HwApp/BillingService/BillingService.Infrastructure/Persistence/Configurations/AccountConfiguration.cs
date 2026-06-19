using BillingService.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    internal class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");

            builder.HasKey(a => a.UserId);

            builder.Property(a => a.UserId)
                .HasColumnName("user_id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(a => a.Balance)
                .HasColumnName("balance")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(a => a.HeldAmount)
                .HasColumnName("held_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Ignore(a => a.AvailableBalance);

            builder.Property(a => a.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(a => a.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
        }
    }
}
