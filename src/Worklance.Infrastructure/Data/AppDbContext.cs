using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.AuthEntities;
using Worklance.Domain.Entities.Job;
namespace Worklance.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<FreelancerProfile> FreelancerProfiles => Set<FreelancerProfile>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<FreelancerEducation> FreelancerEducations => Set<FreelancerEducation>();
    public DbSet<FreelancerCertification> FreelancerCertifications => Set<FreelancerCertification>();
    public DbSet<FreelancerPortfolio> FreelancerPortfolios => Set<FreelancerPortfolio>();
    public DbSet<FreelancerLanguage> FreelancerLanguages => Set<FreelancerLanguage>();
    public DbSet<User> Users { get; set; }
    public DbSet<EmailOtp> EmailOtps { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

    }
}