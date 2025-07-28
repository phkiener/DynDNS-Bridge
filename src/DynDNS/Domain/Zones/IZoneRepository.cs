namespace DynDNS.Domain.Zones;

public interface IZoneRepository
{
    IEnumerable<ZoneConfiguration> GetZones();
    Task UpdateZoneAsync(string zone, Func<ZoneConfiguration, ZoneConfiguration> update, CancellationToken cancellationToken);
    Task DeleteZoneAsync(string zone, CancellationToken cancellationToken);
}
