using System.Security.Claims;
using Worklance.Application;
using Worklance.Infrastructure;
using Worklance.Api.Middleware;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Worklance.Infrastructure.Settings;
using Worklance.Application.Common.ApiResponse;

namespace Worklance.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddMemoryCache();

        // Configure JWT Authentication
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("accessToken"))
                    {
                        context.Token = context.Request.Cookies["accessToken"];
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = async context =>
                {
                    context.NoResult();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    string message = "Please login.";

                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        message = "Session expired. Please login again.";
                    }

                    await context.Response.WriteAsJsonAsync(
                        ApiResponse<object>.Failure(
                            message,
                            StatusCodes.Status401Unauthorized));
                },
                OnChallenge = async context =>
                {
                    if (context.Response.HasStarted)
                        return;
                    context.HandleResponse();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(
                        ApiResponse<object>.Failure(
                            "Please Login",
                            StatusCodes.Status401Unauthorized));
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

        // Wrap DataAnnotation ModelState errors into ApiResponse format
        builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(err =>
                        string.IsNullOrEmpty(err.ErrorMessage)
                            ? $"{e.Key} is invalid."
                            : err.ErrorMessage))
                    .ToList();

                var response = ApiResponse<object>.Failure(
                    "Validation failed.",
                    StatusCodes.Status400BadRequest,
                    errors);

                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
            };
        });
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCors();

        builder.Services.AddSwaggerGen(options =>
        {
            options.OperationFilter<Worklance.Api.Infrastructure.Swagger.FileUploadOperationFilter>();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] {} }
            });
        });

        var app = builder.Build();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Enable CORS
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}