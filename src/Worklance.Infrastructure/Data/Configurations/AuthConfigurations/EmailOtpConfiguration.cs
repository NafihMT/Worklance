using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities.AuthEntities;

namespace Worklance.Infrastructure.Data.Configurations.AuthConfigurations;

public class EmailOtpConfiguration : IEntityTypeConfiguration<EmailOtp>
{
    public void Configure(EntityTypeBuilder<EmailOtp> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OtpCode)
            .IsRequired()
            .HasMaxLength(6);

        builder.HasOne(x => x.User)
            .WithMany(x => x.EmailOtps)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}