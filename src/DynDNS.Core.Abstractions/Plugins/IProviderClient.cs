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
/// A DNS record.
/// </summary>
/// <param name="Hostname">Which domain this record belongs to.</param>
/// <param name="Subdomain">Which subdomain is configured for the domain.</param>
/// <param name="Type">The type of DNS record.</param>
/// <param name="TargetAddress">The IP address the record is pointing at.</param>
public sealed record DnsRecord(Hostname Hostname, DomainFragment Subdomain, DnsRecordType Type, string TargetAddress);

/// <summary>
/// A client to adjust DNS records for a provider.
/// </summary>
/// <seealso cref="IProviderPlugin"/>
public interface IProviderClient
{
    /// <summary>
    /// List all currently configured records.
    /// </summary>
    /// <returns>A list of all found DNS records of supported <see cref="DnsRecordType"/>s.</returns>
    Task<IReadOnlyCollection<DnsRecord>> GetRecordsAsync();

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
