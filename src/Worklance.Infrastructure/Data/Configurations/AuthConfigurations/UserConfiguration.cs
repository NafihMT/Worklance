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
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.AadhaarNumber)
            .IsRequired()
            .HasMaxLength(12);

        builder.HasIndex(x => x.AadhaarNumber)
            .IsUnique();

        builder.Property(x => x.AadhaarImageBytes)
            .IsRequired();

        builder.Property(x => x.AccountType)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();
    }
}