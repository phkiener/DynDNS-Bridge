using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Infrastructure;
using DynDNS.Provider.Abstractions;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="pluginCollection">A collection of all registered <see cref="IProviderPlugin"/>s.</param>
public sealed class ProviderConfigurationsService(ProviderPluginCollection pluginCollection) : IProviderConfigurations
{
    /// <inheritdoc />
    public Task UpdateConfigurationAsync(DomainBindingId id, string key, object? value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<AvailableProvider> AvailableProviders { get; } = pluginCollection.Plugins.Select(Map).ToList();

    private static AvailableProvider Map(IProviderPlugin plugin)
    {
        return new AvailableProvider(plugin.Name, plugin.Parameters);
    }
}
