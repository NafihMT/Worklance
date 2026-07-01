using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Infrastructure.Data;
using Worklance.Infrastructure.Queries;
using Worklance.Infrastructure.Repositories;

namespace Worklance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<DapperContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        // Profile Module Repositories
        services.AddScoped<IFreelancerProfileRepository, FreelancerProfileRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();

        return services;
    }
}
