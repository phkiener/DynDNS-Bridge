using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.Plugins.Hetzner;

/// <summary>
/// A <see cref="IProviderPlugin"/> for the Hetzner DNS console.
/// </summary>
public sealed class HetznerPlugin : IProviderPlugin
{
    /// <inheritdoc />
    public string Name => "Hetzner";

    /// <inheritdoc />
    public IReadOnlyList<string> Parameters { get; } = ["ZoneId", "API Key"];

    public IProviderClient GetClient(IReadOnlyDictionary<string, string> parameters)
    {
        throw new NotImplementedException();
    }
}
