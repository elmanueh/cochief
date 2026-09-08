namespace Cochief.Api.Workers;

using Cochief.Domain.Ports;

internal sealed class ClanSynchronizationWorker(IServiceScopeFactory scopeFactory, ILogger<ClanSynchronizationWorker> logger) : BackgroundService
{
    private const int SynchronizationInterval = 2;

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<ClanSynchronizationWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(SynchronizationInterval));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
                IClanService clanService = scope.ServiceProvider.GetRequiredService<IClanService>();

                await clanService.UpdateAllAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal application shutdown.
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Clan update failed");
            }
        }
    }
}
