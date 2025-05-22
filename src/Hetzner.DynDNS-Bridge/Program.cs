using Hetzner.DynDNSBridge;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddScoped<DnsEntryUpdater>();
builder.Services.AddHttpClient<HetznerApiClient, HetznerApiClient>(HetznerApiClient.ConfigureHttpClient);
builder.Services.AddLogging(static l => l.AddConsole()
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("Microsoft.Hosting", LogLevel.Information)
    .AddFilter("System", LogLevel.Warning)
    .AddFilter("Hetzner.DynDNSBridge", LogLevel.Information));

var host = builder.Build();
host.MapGet("_refresh", Refresh);
await host.RunAsync();

return;

static async Task<IResult> Refresh(
    [FromServices] IConfiguration configuration,
    [FromServices] DnsEntryUpdater entryUpdater,
    [FromQuery(Name = "v4")] string? ipv4,
    [FromQuery(Name = "v6")] string? ipv6,
    CancellationToken cancellationToken)
{
    foreach (var url in configuration.Urls())
    {
        await entryUpdater.UpdateEntryAsync("A", ipv4, url, cancellationToken);
        await entryUpdater.UpdateEntryAsync("AAAA", ipv6, url, cancellationToken);
    }

    return Results.NoContent();
}
