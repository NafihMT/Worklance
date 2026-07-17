using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.HasMany(s => s.Categories)
            .WithMany(c => c.Skills)
            .UsingEntity<Dictionary<string, object>>(
                "CategorySkill",
                j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Skill>().WithMany().HasForeignKey("SkillId").OnDelete(DeleteBehavior.Cascade));
    }
}
