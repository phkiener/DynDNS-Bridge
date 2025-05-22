namespace Hetzner.DynDNSBridge;

/// <summary>
/// A cache for DNS zone information.
/// </summary>
/// <param name="apiClient">The <see cref="HetznerApiClient"/> to use for fetching the DNS zones</param>
public sealed class DnsZoneCache(HetznerApiClient apiClient)
{
    private readonly Dictionary<string, DnsZone?> cachedZones = new();

    /// <summary>
    /// Find a DNS zone with name <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the DNS zone to look up, e.g. <c>foobar.com</c></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    /// <returns>The found DNS zone or null if such a zone does not exist</returns>
    /// <remarks>
    /// If <paramref name="name"/> was resolved before using this instance, the method returns synchronously.
    /// </remarks>
    public async ValueTask<DnsZone?> GetZoneAsync(string name, CancellationToken cancellationToken)
    {
        if (cachedZones.TryGetValue(name, out var cachedZone))
        {
            return cachedZone;
        }
        
        var zone = await apiClient.GetZoneAsync(name, cancellationToken);
        cachedZones[name] = zone;

        return zone;
    }
}
