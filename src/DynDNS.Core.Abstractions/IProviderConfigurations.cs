using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.Abstractions;

/// <summary>
/// Manage and configure the provider that is used to create and update the DNS records of a
/// <em>domain binding</em>.
/// </summary>
public interface IProviderConfigurations
{
    /// <summary>
    /// All available providers.
    /// </summary>
    IReadOnlyCollection<string> Providers { get; }

    /// <summary>
    /// Return the list of required parameters for a provider.
    /// </summary>
    /// <param name="provider">Name of the provider; see <see cref="Providers"/>.</param>
    /// <returns>A list of all parameters that need to be passed to <see cref="ConfigureProviderAsync"/>.</returns>
    IReadOnlyList<string> GetParameters(string provider);

    /// <summary>
    /// Configure a provider for a domain binding.
    /// </summary>
    /// <param name="id">Id of the domain binding whose provider to configure.</param>
    /// <param name="provider">Name of the provider; see <see cref="Providers"/>.</param>
    /// <param name="parameters">The parameters to configure for the selected provider.</param>
    Task ConfigureProviderAsync(DomainBindingId id, string provider, IReadOnlyDictionary<string, string> parameters);

    /// <summary>
    /// Update the bindings for the configured provider.
    /// </summary>
    /// <param name="id">Id of the domain binding whose bindings to update.</param>
    Task UpdateBindingsAsync(DomainBindingId id);

    /// <summary>
    /// Update <em>all</em> configured bindings.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to abort the operation.</param>
    Task UpdateAllBindingsAsync(CancellationToken cancellationToken);
}
