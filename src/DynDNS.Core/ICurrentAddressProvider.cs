namespace DynDNS.Core;

/// <summary>
/// Provides the current public IP address(es).
/// </summary>
public interface ICurrentAddressProvider
{
    /// <summary>
    /// Determine the current public IPv4 address.
    /// </summary>
    /// <returns>The IP address or null.</returns>
    Task<string?> GetIPv4AddressAsync();

    /// <summary>
    /// Determine the current public IPv6 address.
    /// </summary>
    /// <returns>The IP address or null.</returns>
    Task<string?> GetIPv6AddressAsync();
}
