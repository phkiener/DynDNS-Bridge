namespace DynDNS.Zones;

public sealed class ZoneRepository : IZoneRepository
{
    private readonly List<ZoneConfiguration> zones =
    [
        new("phkiener.ch", "Hetzner", true, true, "", ["vpn", "dns", "jellyfin"]),
        new("example.ch", "Hetzner", true, true, "", ["blog"]),
        new("another-one.ch", "Hetzner", true, true, "", ["www", "public", "private"])
    ];

    public IEnumerable<ZoneConfiguration> GetZones()
    {
        return zones.AsEnumerable();
    }

    public Task UpdateZoneAsync(string zone, Func<ZoneConfiguration, ZoneConfiguration> update, CancellationToken cancellationToken)
    {
        var index = zones.FindIndex(f => f.Zone == zone);
        if (index is not -1)
        {
            zones[index] = update(zones[index]);
        }
        
        return Task.CompletedTask;
    }

    public Task DeleteZoneAsync(string zone, CancellationToken cancellationToken)
    {
        var foundZone = zones.SingleOrDefault(z => z.Zone == zone);
        if (foundZone is not null)
        {
            zones.Remove(foundZone);
        }
        
        return Task.CompletedTask;
    }
}
