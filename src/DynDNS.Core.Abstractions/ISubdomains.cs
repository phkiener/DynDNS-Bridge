using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.Abstractions;

/// <summary>
/// Manage the subdomains to bind on a <em>domain binding</em>.
/// </summary>
public interface ISubdomains
{
    /// <summary>
    /// Add a new subdomain to the specified domain binding.
    /// </summary>
    /// <param name="id">Id of the domain binding to which to add the subdomain.</param>
    /// <param name="name">The <see cref="DomainFragment"/> to configure as subdomain.</param>
    Task AddSubdomainAsync(DomainBindingId id, DomainFragment name);

    /// <summary>
    /// Update the binding flags for the specified subdomain.
    /// </summary>
    /// <param name="id">Id of the domain binding to which to add the subdomain.</param>
    /// <param name="name">The subdomain to configure.</param>
    /// <param name="flags">The flags to set on the subdomain.</param>
    Task UpdateSubdomainAsync(DomainBindingId id, DomainFragment name, SubdomainFlags flags);
    
    /// <summary>
    /// Remove a subdomain from the specified domain binding. If any binding is configured, all
    /// matching DNS records for this subdomain will be deleted from the provider.
    /// </summary>
    /// <param name="id">Id of the domain binding from which to remove the subdomain.</param>
    /// <param name="name">The subdomain to remove.</param>
    Task RemoveSubdomainAsync(DomainBindingId id, DomainFragment name);
}
