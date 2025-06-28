namespace DynDNS.Zones;

public sealed class ZoneRepository : IZoneRepository
{
    private readonly List<ZoneConfiguration> zones =
    [
        new("phkiener.ch", "Hetzner", new Dictionary<string, string> { ["API Key"] = "fQwa..." }, ["vpn", "dns", "jellyfin"]),
        new("example.ch", "Hetzner", new Dictionary<string, string> { ["API Key"] = "av34..." }, ["blog"]),
        new("another-one.ch", "Hetzner", new Dictionary<string, string> { ["API Key"] = "mmh4..." }, ["www", "public", "private"])
    ];

    public IEnumerable<ZoneConfiguration> GetZones()
    {
        return zones.AsEnumerable();
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
