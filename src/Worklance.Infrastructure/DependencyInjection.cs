using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Worklance.Application.Interfaces.Queries;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Application.Interfaces.Services;
using Worklance.Infrastructure.Data;
using Worklance.Infrastructure.Queries;
using Worklance.Infrastructure.Repositories;
using Worklance.Infrastructure.Services;

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

        // Profile Module Queries
        services.AddScoped<IFreelancerProfileQueryService, FreelancerProfileQueryService>();

        // Profile Module Storage Services
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddScoped<IAdminUserVerificationQuery, AdminUserVerificationQuery>();

        // --- Missing Auth Services from the merge conflict ---
        services.Configure<Worklance.Application.Common.Settings.CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.Configure<Worklance.Application.Common.Settings.EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<Worklance.Infrastructure.Settings.JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<Worklance.Application.Interfaces.AuthInterface.IAuthRepository, Worklance.Infrastructure.Data.Repositories.AuthRepo.AuthRepository>();
        services.AddScoped<Worklance.Application.Interfaces.AuthInterface.IAuthService, Worklance.Infrastructure.Services.AuthServices.AuthService>();
        services.AddScoped<Worklance.Application.Interfaces.CloudinaryInterface.ICloudinaryService, Worklance.Infrastructure.Services.Cloudinary.CloudinaryService>();
        services.AddScoped<Worklance.Application.Interfaces.EmailInterface.IEmailService, Worklance.Infrastructure.Services.Email.EmailService>();
        services.AddScoped<Worklance.Application.Interfaces.AuthInterface.IJwtService, Worklance.Infrastructure.Services.JWT.JwtService>();
        services.AddScoped<Worklance.Application.Interfaces.AuthInterface.IOcrService, Worklance.Infrastructure.Services.OCR.OcrService>();
        services.AddScoped<Worklance.Application.Interfaces.Repositories.IUserVerificationRepository, Worklance.Infrastructure.Repositories.UserVerificationRepository>();

        return services;
    }
}
