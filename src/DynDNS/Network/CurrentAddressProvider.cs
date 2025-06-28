namespace DynDNS.Network;

public sealed class CurrentAddressProvider(ILogger<CurrentAddressProvider> logger, IHttpClientFactory httpClientFactory)
    : BackgroundService, ICurrentAddressProvider
{
    private PeriodicTimer? timer;
    
    public string? IPv4 { get; private set; }
    public string? IPv6 { get; private set; }
    
    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await RefreshCurrentAddressAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (!stoppingToken.IsCancellationRequested)
        {
            await RefreshCurrentAddressAsync(stoppingToken);
            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task RefreshCurrentAddressAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(nameof(CurrentAddressProvider));
            IPv4 = await DetermineIPv4Async(client, cancellationToken);
            IPv6 = await DetermineIPv6Async(client, cancellationToken);
        }
        catch (Exception e) when (e is not (TaskCanceledException or OperationCanceledException))
        {
            logger.LogError(e, "Error while refreshing current address");
        }
    }

    private static async Task<string?> DetermineIPv4Async(HttpClient client, CancellationToken cancellationToken)
    {
        var response = await client.GetAsync("https://4.icanhazip.com/", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();
        }

        return null;
    }

    private static async Task<string?> DetermineIPv6Async(HttpClient client, CancellationToken cancellationToken)
    {
        var response = await client.GetAsync("https://6.icanhazip.com/", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();
        }

        return null;
    }

    public override void Dispose()
    {
        timer?.Dispose();
        timer = null;

        base.Dispose();
    }
}
