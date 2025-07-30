using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.Abstractions;

/// <summary>
/// Manage the provider-specific configuration of a <em>domain binding</em>.
/// </summary>
public interface IProviderConfigurations
{
    /// <summary>
    /// Update a specific configuration parameter.
    /// </summary>
    /// <param name="id">Id of the domain binding whose configuration to adjust.</param>
    /// <param name="key">Name of the configuration parameter to update.</param>
    /// <param name="value">The updated value to store.</param>
    Task UpdateConfigurationAsync(DomainBindingId id, string key, object? value);
    
    /// <summary>
    /// All providers that are available for <em>domain bindings</em>.
    /// </summary>
    IReadOnlyCollection<AvailableProvider> AvailableProviders { get; }
}
