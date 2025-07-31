using DynDNS.Core.Abstractions.Models;
using DynDNS.Provider.Abstractions;

namespace DynDNS.Provider.Hetzner;

/// <summary>
/// A <see cref="IProviderPlugin"/> for Hetzner.
/// </summary>
public sealed class HetznerProviderPlugin : IProviderPlugin
{
    /// <inheritdoc />
    public string Name => "Hetzner";

    /// <inheritdoc />
    public IReadOnlyCollection<ProviderConfigurationParameter> Parameters { get; } =
    [
        new("Zone Id", true, ParameterType.Text),
        new("API Key", true, ParameterType.Text),
    ];
}
