using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Provider.Abstractions;

/// <summary>
/// A provider that may be used to manage DNS records.
/// </summary>
public interface IProviderPlugin
{
    /// <summary>
    /// Name of the provider.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Parameters required for configuration.
    /// </summary>
    IReadOnlyCollection<ProviderConfigurationParameter> Parameters { get; }
}
