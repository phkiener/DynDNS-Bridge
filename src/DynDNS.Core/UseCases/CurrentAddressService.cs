using DynDNS.Core.Abstractions;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="currentAddressProvider">The <see cref="ICurrentAddressProvider"/> which will be used.</param>
public sealed class CurrentAddressService(ICurrentAddressProvider currentAddressProvider) : ICurrentAddress
{
    /// <inheritdoc />
    public string? IPv4Address { get; private set; }

    /// <inheritdoc />
    public string? IPv6Address { get; private set; }

    /// <inheritdoc />
    public async Task RefreshAsync()
    {
        IPv4Address = await currentAddressProvider.GetIPv4AddressAsync();
        IPv6Address = await currentAddressProvider.GetIPv6AddressAsync();

        AddressChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public event EventHandler? AddressChanged;
}
