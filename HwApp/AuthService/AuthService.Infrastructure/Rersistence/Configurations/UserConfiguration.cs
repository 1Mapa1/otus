using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Rersistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(x => x.Login)
                .IsRequired()
                .HasColumnName("login")
                .HasMaxLength(50);

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasColumnName("password_hash");

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasColumnName("status");

            builder.Property(x => x.CreateAt)
                .IsRequired()
                .HasColumnName("create_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
