namespace DynDNS.Core.Abstractions.Models;

/// <summary>
/// A read-only view of a <em>domain binding</em>.
/// </summary>
/// <param name="Id">Id of the domain binding.</param>
/// <param name="Domain">The common hostname of this binding.</param>
/// <param name="Subdomains">The subdomains configured for this binding.</param>
/// <param name="Provider">The provider used to manage DNS records.</param>
/// <param name="ProviderConfigurationParameters">Configuration parameters used for the provider.</param>
/// <remarks>
/// To make sense of the <see cref="ProviderConfigurationParameters"/>, see the
/// <see cref="AvailableProvider.ConfigurationParameters"/> in <see cref="AvailableProvider"/> retrieved
/// via <see cref="IProviderConfigurations"/>.
/// </remarks>
public sealed record DomainBinding(
    DomainBindingId Id,
    string Provider,
    Hostname Domain,
    DomainBinding.Subdomain[] Subdomains,
    IReadOnlyDictionary<string, object> ProviderConfigurationParameters)
{
    /// <summary>
    /// A subdomain as part of a <see cref="DomainBinding"/>.
    /// </summary>
    /// <param name="Name">Name of the subdomain.</param>
    /// <param name="Flags">The configured flags for the subdomain.</param>
    public sealed record Subdomain(DomainFragment Name, SubdomainFlags Flags);
}
