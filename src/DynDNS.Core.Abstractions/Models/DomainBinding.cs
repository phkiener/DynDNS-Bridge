namespace DynDNS.Core.Abstractions.Models;

/// <summary>
/// A read-only view of a <em>domain binding</em>.
/// </summary>
/// <param name="Id">Id of the domain binding.</param>
/// <param name="Domain">The common hostname of this binding.</param>
/// <param name="Subdomains">The subdomains configured for this binding.</param>
public sealed record DomainBinding(
    DomainBindingId Id,
    Hostname Domain,
    DomainBinding.Subdomain[] Subdomains)
{
    /// <summary>
    /// A subdomain as part of a <see cref="DomainBinding"/>.
    /// </summary>
    /// <param name="Name">Name of the subdomain.</param>
    /// <param name="Flags">The configured flags for the subdomain.</param>
    public sealed record Subdomain(DomainFragment Name, SubdomainFlags Flags);
}
