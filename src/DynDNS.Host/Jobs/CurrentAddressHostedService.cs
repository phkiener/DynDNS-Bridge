using DynDNS.Core.Abstractions;

namespace DynDNS.Host.Jobs;

public sealed class CurrentAddressHostedService(IServiceProvider serviceProvider) : BackgroundService
{
    private PeriodicTimer? timer;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await RefreshCurrentAddressAsync();
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (!stoppingToken.IsCancellationRequested)
        {
            await timer.WaitForNextTickAsync(stoppingToken);
            await RefreshCurrentAddressAsync();
        }
    }

    private async Task RefreshCurrentAddressAsync()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var currentAddressService = scope.ServiceProvider.GetRequiredService<ICurrentAddress>();
        await currentAddressService.RefreshAsync();
    }

    public override void Dispose()
    {
        timer?.Dispose();
        timer = null;

        base.Dispose();
    }
}
