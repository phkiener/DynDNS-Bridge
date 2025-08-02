using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.Model;

/// <summary>
/// A subdomain as part of a <see cref="DomainBinding"/>.
/// </summary>
/// <param name="name">Name of this subdomain.</param>
/// <param name="createIPv4Record">Whether to create a DNS entry for IPv4 routing.</param>
/// <param name="createIPv6Record">Whether to create a DNS entry for IPv6 routing.</param>
public sealed class Subdomain(DomainFragment name, bool createIPv4Record = false, bool createIPv6Record = false)
{
    /// <summary>
    /// Name of this subdomain.
    /// </summary>
    public DomainFragment Name { get; } = name;

    /// <summary>
    /// Whether to create a DNS entry for IPv4 routing.
    /// </summary>
    public bool CreateIPv4Record { get; private set; } = createIPv4Record;

    /// <summary>
    /// Whether to create a DNS entry for IPv6 routing.
    /// </summary>
    public bool CreateIPv6Record { get; private set; } = createIPv6Record;

    /// <summary>
    /// Apply the given flags to the subdomain, modifying its configuration.
    /// </summary>
    /// <param name="flags">The flags to apply.</param>
    public void ApplyFlags(SubdomainFlags flags)
    {
        CreateIPv4Record = flags.HasFlag(SubdomainFlags.A);
        CreateIPv6Record = flags.HasFlag(SubdomainFlags.AAAA);
    }
}
