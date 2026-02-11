using DynDNS.Core.Abstractions.Plugins;
using Microsoft.Extensions.Logging;

namespace DynDNS.Core.Plugins.HetznerCloud;

/// <summary>
/// A <see cref="IProviderPlugin"/> for the Hetzner Cloud DNS API.
/// </summary>
public sealed class HetznerCloudPlugin(IHttpClientFactory httpClientFactory, ILogger<HetznerCloudPlugin> logger) : IProviderPlugin
{
    /// <inheritdoc />
    public string Name => "Hetzner Cloud";

    /// <inheritdoc />
    public IReadOnlyList<string> Parameters { get; } = ["Label", "API Key"];

    public IProviderClient GetClient(IReadOnlyDictionary<string, string> parameters)
    {
        var httpClient = httpClientFactory.CreateClient(nameof(HetznerCloudClient));
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {parameters["ApiKey"]}");

        return new HetznerCloudClient(httpClient, parameters["Label"], logger);
    }
}
