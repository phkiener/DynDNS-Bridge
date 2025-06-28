namespace DynDNS.Network;

public interface ICurrentAddressProvider
{
    string? IPv4 { get; }
    string? IPv6 { get; }
    
    Task RefreshAsync(CancellationToken cancellationToken);
}
