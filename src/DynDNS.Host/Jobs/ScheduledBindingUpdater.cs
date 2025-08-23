using DynDNS.Core.Abstractions;

namespace DynDNS.Host.Jobs;

public sealed class ScheduledBindingUpdater(
    IConfiguration configuration,
    ILogger<ScheduledBindingUpdater> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    private PeriodicTimer? timer;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await UpdateBindingsAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = configuration.GetValue<int>("DYNDNS_SCHEDULE");
        timer = new PeriodicTimer(TimeSpan.FromMinutes(interval));

        while (!stoppingToken.IsCancellationRequested)
        {
            await timer.WaitForNextTickAsync(stoppingToken);

            logger.LogInformation("Running scheduled update on all bindings");
            await UpdateBindingsAsync(stoppingToken);
        }
    }

    private async Task UpdateBindingsAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var currentAddressService = scope.ServiceProvider.GetRequiredService<IProviderConfigurations>();
        await currentAddressService.UpdateAllBindingsAsync(stoppingToken);
    }

    public override void Dispose()
    {
        timer?.Dispose();
        timer = null;

        base.Dispose();
    }
}
