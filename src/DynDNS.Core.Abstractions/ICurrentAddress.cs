namespace DynDNS.Core.Abstractions;

/// <summary>
/// Provides access to the current public IP address.
/// </summary>
public interface ICurrentAddress
{
    /// <summary>
    /// The current public IPv4 address, if any.
    /// </summary>
    public string? IPv4Address { get; }

    /// <summary>
    /// The current public IPv6 address, if any.
    /// </summary>
    public string? IPv6Address { get; }

    /// <summary>
    /// Refresh the public IP addresses.
    /// </summary>
    Task RefreshAsync();
}
