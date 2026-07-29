using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Worklance.Domain.Entities;

namespace Worklance.Infrastructure.Persistence.Configurations;

public class FreelancerLanguageConfiguration : IEntityTypeConfiguration<FreelancerLanguage>
{
    public void Configure(EntityTypeBuilder<FreelancerLanguage> builder)
    {
        builder.ToTable("FreelancerLanguages");

        builder.HasKey(fl => fl.Id);

        builder.Property(fl => fl.LanguageName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fl => fl.Proficiency)
            .IsRequired();
    }
}
