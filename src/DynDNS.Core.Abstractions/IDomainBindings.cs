using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.Abstractions;

/// <summary>
/// Get and modify any <em>domain binding</em>.
/// </summary>
public interface IDomainBindings
{
    /// <summary>
    /// Create a new domain binding for <see cref="domain"/> using provider <see cref="provider"/>.
    /// </summary>
    /// <param name="domain">The <see cref="Hostname"/> to bind.</param>
    /// <param name="provider">The provider to use when binding the hostname.</param>
    /// <returns>The <see cref="DomainBindingId"/> for the newly created domain binding.</returns>
    /// <remarks>
    /// Use <see cref="IProviderConfigurations"/> to set up the required parameters for the provider.
    /// Use <see cref="ISubdomains"/> to manage the subdomains to bind for this domain.
    /// </remarks>
    Task<DomainBindingId> CreateDomainBindingAsync(Hostname domain, string provider);
    
    /// <summary>
    /// Remove a domain binding, including all subdomains and provider configuration parameters.
    /// If any subdomains are configured, all matching DNS records will be deleted from the provider.
    /// </summary>
    /// <param name="id">Id of the domain binding to remove.</param>
    Task RemoveDomainBindingAsync(DomainBindingId id);

    /// <summary>
    /// Retrieve all configured domain bindings.
    /// </summary>
    /// <returns>A list of all found domain bindings.</returns>
    Task<IReadOnlyCollection<DomainBinding>> GetDomainBindingsAsync();
    
    /// <summary>
    /// Find a domain binding by its hostname.
    /// </summary>
    /// <param name="hostname">The hostname whose domain binding to find</param>
    /// <returns>The found domain binding or <c>null</c>.</returns>
    Task<DomainBinding?> FindDomainBindingAsync(Hostname hostname);
}
