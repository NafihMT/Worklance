using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace Worklance.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register all validators in the assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
