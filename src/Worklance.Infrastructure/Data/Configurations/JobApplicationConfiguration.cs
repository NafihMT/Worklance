using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities.Job;

namespace Worklance.Infrastructure.Data.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CoverLetterBytes)
            .IsRequired();

        builder.Property(a => a.CoverLetterFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.CoverLetterContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.ProposedRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.EstimatedDays)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(a => new { a.JobId, a.FreelancerProfileId })
            .IsUnique();

        builder.HasOne(a => a.Job)
            .WithMany(j => j.JobApplications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.FreelancerProfile)
            .WithMany(f => f.JobApplications)
            .HasForeignKey(a => a.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
