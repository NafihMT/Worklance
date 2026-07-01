using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Worklance.Domain.Entities;

namespace Worklance.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<FreelancerProfile> FreelancerProfiles => Set<FreelancerProfile>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<FreelancerEducation> FreelancerEducations => Set<FreelancerEducation>();
    public DbSet<FreelancerCertification> FreelancerCertifications => Set<FreelancerCertification>();
    public DbSet<FreelancerPortfolio> FreelancerPortfolios => Set<FreelancerPortfolio>();
    public DbSet<FreelancerLanguage> FreelancerLanguages => Set<FreelancerLanguage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
