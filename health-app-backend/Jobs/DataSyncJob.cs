using health_app_backend.Repositories;
using health_app_backend.Services;

namespace health_app_backend.Jobs;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class DataSyncJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataSyncJob> _logger;

    public DataSyncJob(IServiceProvider serviceProvider, ILogger<DataSyncJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nowUtc = DateTime.UtcNow;
                
            // Target time: 3 AM UTC
            var nextRunTime = nowUtc.Date.AddDays(1).AddHours(3);

            var delay = nextRunTime - nowUtc;

            Console.WriteLine($"[DataSyncJob] Next run at: {nextRunTime} UTC");
            _logger.LogInformation("[DataSyncJob] Next run at: {NextRunTime} UTC", nextRunTime);

            await Task.Delay(delay, stoppingToken);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var healthDataService = scope.ServiceProvider.GetRequiredService<IHealthDataService>();

                    var users = await userRepository.GetAllAsync();

                    foreach (var user in users)
                    {
                        _logger.LogInformation("[DataSyncJob] Starting data sync for {UserId}", user.Id);
                        await healthDataService.SyncYesterdayDataAsync(user.Id, DateTime.UtcNow);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DataSyncJob] Error occurred during data sync.");
                Console.WriteLine($"[DataSyncJob] Error: {ex.Message}");
            }
        }
    }
}