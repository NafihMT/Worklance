using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class FreelancerProfileConfiguration : IEntityTypeConfiguration<FreelancerProfile>
{
    public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
    {
        builder.ToTable("FreelancerProfiles");

        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(fp => fp.UserId)
            .IsUnique();

        builder.Property(fp => fp.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fp => fp.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fp => fp.Username)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(fp => fp.Username)
            .IsUnique();

        builder.Property(fp => fp.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(fp => fp.Email)
            .IsUnique();

        builder.Property(fp => fp.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(fp => fp.ProfilePhotoUrl)
            .HasMaxLength(2083);

        builder.Property(fp => fp.Country)
            .HasMaxLength(100);

        builder.Property(fp => fp.State)
            .HasMaxLength(100);

        builder.Property(fp => fp.City)
            .HasMaxLength(100);

        builder.Property(fp => fp.Address)
            .HasMaxLength(500);

        builder.Property(fp => fp.ProfessionalTitle)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(fp => fp.AboutMe)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(fp => fp.PrimaryTechnologyStack)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fp => fp.Specialization)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fp => fp.HourlyRate)
            .HasPrecision(18, 2);

        builder.Property(fp => fp.ResumeUrl)
            .HasMaxLength(2083);

        builder.Property(fp => fp.GitHubUrl)
            .HasMaxLength(2083);

        builder.Property(fp => fp.LinkedInUrl)
            .HasMaxLength(2083);

        builder.Property(fp => fp.PortfolioWebsiteUrl)
            .HasMaxLength(2083);

        builder.Property(fp => fp.TwitterUrl)
            .HasMaxLength(2083);

        // Configure Many-to-Many with Skill
        builder.HasMany(fp => fp.Skills)
            .WithMany(s => s.FreelancerProfiles)
            .UsingEntity<Dictionary<string, object>>(
                "FreelancerSkill",
                j => j.HasOne<Skill>().WithMany().HasForeignKey("SkillId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<FreelancerProfile>().WithMany().HasForeignKey("FreelancerProfileId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("FreelancerSkills");
                    j.HasKey("FreelancerProfileId", "SkillId");
                });

        // Configure One-to-Many Relationships
        builder.HasMany(fp => fp.Educations)
            .WithOne(e => e.FreelancerProfile)
            .HasForeignKey(e => e.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(fp => fp.Certifications)
            .WithOne(c => c.FreelancerProfile)
            .HasForeignKey(c => c.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(fp => fp.Portfolios)
            .WithOne(p => p.FreelancerProfile)
            .HasForeignKey(p => p.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(fp => fp.Languages)
            .WithOne(l => l.FreelancerProfile)
            .HasForeignKey(l => l.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
