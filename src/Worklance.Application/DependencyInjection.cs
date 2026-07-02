using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Worklance.Application.Interfaces.Services;
using Worklance.Application.Mapping.FreelancerProfileMapping;
using Worklance.Application.Services;

namespace Worklance.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<FreelancerProfileMappingProfile>();
        });

        services.AddScoped<IFreelancerProfileService, FreelancerProfileService>();


        services.AddScoped<IJobService, JobService>();

        return services;
    }
}
