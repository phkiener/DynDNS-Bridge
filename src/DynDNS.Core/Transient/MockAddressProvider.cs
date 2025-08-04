namespace DynDNS.Core.Transient;

internal sealed class MockAddressProvider : ICurrentAddressProvider
{
    public string? IPv4Address { get; set; } = "127.0.0.1";
    public string? IPv6Address { get; set; } = "0:0:0:0:0:0:0:1";

    public Task<string?> GetIPv4AddressAsync()
    {
        return Task.FromResult(IPv4Address);
    }

    public Task<string?> GetIPv6AddressAsync()
    {
        return Task.FromResult(IPv6Address);
    }
}
