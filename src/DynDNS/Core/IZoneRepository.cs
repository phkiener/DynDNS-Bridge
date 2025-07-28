using DynDNS.Core.Domains;

namespace DynDNS.Core;

public interface IZoneRepository
{
    IEnumerable<ZoneConfiguration> GetZones();
    Task UpdateZoneAsync(string zone, Func<ZoneConfiguration, ZoneConfiguration> update, CancellationToken cancellationToken);
    Task DeleteZoneAsync(string zone, CancellationToken cancellationToken);
}
