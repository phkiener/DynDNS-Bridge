using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.Abstractions.Plugins;

/// <summary>
/// The type of DNS record.
/// </summary>
public enum DnsRecordType
{
    /// <summary>
    /// An A record for IPv4 addresses.
    /// </summary>
    A,

    /// <summary>
    /// An AAAA record for IPv6 addresses.
    /// </summary>
    AAAA
}

/// <summary>
/// A client to adjust DNS records for a provider.
/// </summary>
/// <seealso cref="IProviderPlugin"/>
public interface IProviderClient
{
    /// <summary>
    /// Update (or create) a specific record.
    /// </summary>
    /// <param name="hostname">The hostname for which to update the record.</param>
    /// <param name="subdomain">The subdomain to configure.</param>
    /// <param name="type">The type of record to update.</param>
    /// <param name="ipAddress">The target IP address to set for the record.</param>
    Task CreateRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type, string ipAddress);

    /// <summary>
    /// Remove a DNS record.
    /// </summary>
    /// <param name="hostname">The hostname for which to remove the record.</param>
    /// <param name="subdomain">The subdomain to configure.</param>
    /// <param name="type">The type of record to remove.</param>
    Task DeleteRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type);
}
