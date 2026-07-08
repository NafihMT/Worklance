using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(j => j.Description)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(j => j.FixedBudget)
            .HasColumnType("decimal(18,2)");

        builder.Property(j => j.MinHourlyRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(j => j.MaxHourlyRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(j => j.TagsJson)
            .HasColumnName("Tags")
            .HasColumnType("nvarchar(max)");

        builder.Property(j => j.JobType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(j => j.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(j => j.ClientProfile)
            .WithMany()
            .HasForeignKey(j => j.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Category)
            .WithMany()
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete query filter
        builder.HasQueryFilter(j => !j.IsDeleted);  // No Soft delete in Global Filter 

        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => j.ClientProfileId);
    }
}