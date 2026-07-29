using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class FreelancerPortfolioConfiguration : IEntityTypeConfiguration<FreelancerPortfolio>
{
    public void Configure(EntityTypeBuilder<FreelancerPortfolio> builder)
    {
        builder.ToTable("FreelancerPortfolios");

        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(fp => fp.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(fp => fp.ProjectUrl)
            .HasMaxLength(2083);

        builder.Property(fp => fp.ImageUrl)
            .HasMaxLength(2083);

        builder.Property(fp => fp.TechnologiesUsed)
            .HasMaxLength(500);
    }
}
