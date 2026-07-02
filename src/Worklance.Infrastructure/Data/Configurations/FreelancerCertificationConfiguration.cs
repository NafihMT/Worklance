using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class FreelancerCertificationConfiguration : IEntityTypeConfiguration<FreelancerCertification>
{
    public void Configure(EntityTypeBuilder<FreelancerCertification> builder)
    {
        builder.ToTable("FreelancerCertifications");

        builder.HasKey(fc => fc.Id);

        builder.Property(fc => fc.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(fc => fc.IssuingOrganization)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(fc => fc.CredentialId)
            .HasMaxLength(100);

        builder.Property(fc => fc.CredentialUrl)
            .HasMaxLength(2083);
    }
}
