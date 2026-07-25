using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using Worklance.Infrastructure.Queries; 

namespace Worklance.Api.BackgroundJobs
{
    public class UnverifiedUserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UnverifiedUserCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dapperContext = scope.ServiceProvider.GetRequiredService<DapperContext>();
                        var cutoffTime = DateTime.UtcNow.AddMinutes(-30);

                        var sql = "DELETE FROM Users WHERE EmailVerified = 0 AND CreatedAt < @Cutoff";

                        using var connection = dapperContext.CreateConnection();
                        await connection.ExecuteAsync(sql, new { Cutoff = cutoffTime });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BACKGROUND JOB ERROR]: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}