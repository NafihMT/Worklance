using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities.AuthEntities;

namespace Worklance.Infrastructure.Data.Configurations.AuthConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PhoneNumber)
            .HasConversion(
                v => v.ToString(),
                v => string.IsNullOrEmpty(v) ? 0L : Convert.ToInt64(v))
            .IsRequired();

        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.AadhaarNumber)
            .HasConversion(
                v => v.ToString(),
                v => string.IsNullOrEmpty(v) ? 0L : Convert.ToInt64(v))
            .IsRequired();

        builder.HasIndex(x => x.AadhaarNumber)
            .IsUnique();

        builder.Property(x => x.AadhaarImageBytes)
            .IsRequired();

        builder.Property(x => x.AccountType)
            .HasConversion<int>();

        builder.Property(x => x.Role)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.AdminVerificationStatus)
            .HasConversion<int>();
    }
}