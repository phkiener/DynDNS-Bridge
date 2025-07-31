using DynDNS.Provider.Abstractions;

namespace DynDNS.Core.Infrastructure;

/// <summary>
/// A collection of all registered <see cref="IProviderPlugin"/>s.
/// </summary>
/// <param name="providerPlugins">The plugins, received via DI container.</param>
public sealed class ProviderPluginCollection(IEnumerable<IProviderPlugin> providerPlugins)
{
    private readonly IReadOnlyDictionary<string, IProviderPlugin> plugins = providerPlugins.ToDictionary(static x => x.Name);

    /// <summary>
    /// Enumerate all registered plugins.
    /// </summary>
    public IEnumerable<IProviderPlugin> Plugins => plugins.Values;

    /// <summary>
    /// Find a specific plugin by name.
    /// </summary>
    /// <param name="name">Name of the plugin to retrieve.</param>
    /// <returns>The found plugin or <c>null</c>.</returns>
    public IProviderPlugin? GetPlugin(string name) => plugins.GetValueOrDefault(name);
}
