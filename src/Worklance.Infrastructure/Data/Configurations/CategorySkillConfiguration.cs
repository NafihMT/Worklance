using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities.Job;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class CategorySkillConfiguration : IEntityTypeConfiguration<CategorySkill>
{
    public void Configure(EntityTypeBuilder<CategorySkill> builder)
    {
        builder.ToTable("CategorySkills");

        builder.HasKey(cs => new { cs.CategoryId, cs.SkillId });

        builder.HasOne(cs => cs.Category)
            .WithMany(c => c.CategorySkills)
            .HasForeignKey(cs => cs.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Skill)
            .WithMany(s => s.CategorySkills)
            .HasForeignKey(cs => cs.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}