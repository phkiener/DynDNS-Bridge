using DynDNS.Framework;
using DynDNS.WebInterface.Components;
using DynDNS.Zones;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DynDNS;

public static class Endpoints
{
    public static async Task<IResult> RefreshAddress([FromServices] ICurrentAddressProvider provider, CancellationToken cancellationToken)
    {
        await provider.RefreshAsync(cancellationToken);

        return new RazorComponentResult<CurrentConnection>();
    }
    
    public static async Task<IResult> DeleteZone(
        [FromRoute(Name = "name")] string zone,
        [FromServices] IZoneRepository zoneRepository,
        CancellationToken cancellationToken)
    {
        await zoneRepository.DeleteZoneAsync(zone, cancellationToken);

        return Results.Empty;
    }
}
