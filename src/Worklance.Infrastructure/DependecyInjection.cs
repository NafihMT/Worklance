using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Worklance.Application.Interfaces.AuthInterface;
using Worklance.Application.Interfaces.CloudinaryInterface;
using Worklance.Application.Interfaces.EmailInterface;
using Worklance.Infrastructure.Data;
using Worklance.Infrastructure.Data.Repositories.AuthRepo;
using Worklance.Infrastructure.Services.AuthServices;
using Worklance.Infrastructure.Services.Cloudinary;
using Worklance.Infrastructure.Services.Email;
using Worklance.Infrastructure.Services.JWT;
using Worklance.Infrastructure.Services.OCR;
using Worklance.Application.Common.Settings;
using Worklance.Infrastructure.Settings;

namespace Worklance.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Settings Options
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();

            // Register Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOcrService, OcrService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
