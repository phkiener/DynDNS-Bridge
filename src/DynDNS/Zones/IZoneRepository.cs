namespace DynDNS.Zones;

public interface IZoneRepository
{
    IEnumerable<ZoneConfiguration> GetZones();

    Task DeleteZoneAsync(string zone, CancellationToken cancellationToken);
}
