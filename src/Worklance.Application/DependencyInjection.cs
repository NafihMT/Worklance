using System.Reflection;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Worklance.Application.Interfaces.Services;
using Worklance.Application.Mapping.FreelancerProfileMapping;
using Worklance.Application.Services;
using Worklance.Application.Services.Jobs;

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

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IJobApplicationService, JobApplicationService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}
