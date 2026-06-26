using Worklance.Api.Infrastructure;
using Worklance.Api.Services;
using Worklance.Application;
using Worklance.Application.Common.Interfaces;
using Worklance.Infrastructure;
using Worklance.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services from the Clean Architecture layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add API presentation layer services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Register global exception handling
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Initialise and seed the database automatically in development
    using var scope = app.Services.CreateScope();
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

// Add global exception handling middleware
app.UseExceptionHandler();

app.UseHttpsRedirection();

// Root endpoint to verify API health/running status
app.MapGet("/", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Project = "Worklance API", 
    Environment = app.Environment.EnvironmentName 
}));

app.Run();
