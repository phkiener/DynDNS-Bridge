namespace DynDNS.Framework;

public interface ICurrentAddressProvider
{
    string? IPv4 { get; }
    string? IPv6 { get; }
    
    Task RefreshAsync(CancellationToken cancellationToken);
}
