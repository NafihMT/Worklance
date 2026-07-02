using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class FreelancerEducationConfiguration : IEntityTypeConfiguration<FreelancerEducation>
{
    public void Configure(EntityTypeBuilder<FreelancerEducation> builder)
    {
        builder.ToTable("FreelancerEducations");

        builder.HasKey(fe => fe.Id);

        builder.Property(fe => fe.Institution)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(fe => fe.Degree)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(fe => fe.FieldOfStudy)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(fe => fe.Description)
            .HasMaxLength(1000);
    }
}
