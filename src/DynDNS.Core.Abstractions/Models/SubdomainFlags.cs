namespace DynDNS.Core.Abstractions.Models;

/// <summary>
/// Configuration flags for a subdomain.
/// </summary>
[Flags]
public enum SubdomainFlags
{
    /// <summary>
    /// Create no records at all.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Create an A-record (for IPv4) when binding the subdomain.
    /// </summary>
    A = 1,
    
    /// <summary>
    /// Create an AAAA-record (for IPv6) when binding the subdomain.
    /// </summary>
    AAAA = 2
}
