namespace DynDNS.Framework;

public interface ICurrentAddressProvider
{
    string? IPv4 { get; }
    string? IPv6 { get; }

    event EventHandler AddressChanged;
}
