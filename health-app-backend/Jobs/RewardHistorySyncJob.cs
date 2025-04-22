using health_app_backend.Services;

namespace health_app_backend.Jobs;

public class RewardHistorySyncJob : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<RewardHistorySyncJob> _logger;

    public RewardHistorySyncJob(IServiceProvider sp, ILogger<RewardHistorySyncJob> logger)
    {
        _sp      = sp;
        _logger  = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stop)
    {
        while (!stop.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var bcSvc = scope.ServiceProvider.GetRequiredService<IBlockchainService>();

                await bcSvc.SyncPointsRewardHistoryFromChainAsync();
                await bcSvc.SyncTokenRewardHistoryFromChainAsync();

                _logger.LogInformation("✅ Reward‑history sync completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Reward‑history sync failed");
            }

            // wait e.g. 1 hour between polls
            await Task.Delay(TimeSpan.FromHours(1), stop);
        }
    }
}