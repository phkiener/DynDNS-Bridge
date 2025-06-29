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

    public static IResult AddZoneTemplate()
    {
        var parameters = new Dictionary<string, object?>
        {
            [nameof(Zone.Name)] = "",
            [nameof(Zone.Provider)] = "",
            [nameof(Zone.Parameters)] = new Dictionary<string, string>(),
            [nameof(Zone.Subdomains)] = Array.Empty<string>()
        };
        
        return new RazorComponentResult<ZoneTemplate>(parameters);
    }

    public static IResult AddZone(
        [FromForm] bool? save,
        [FromForm] string? name,
        [FromForm] string? provider)
    {
        var parameters = new Dictionary<string, object?>
        {
            [nameof(Zone.Name)] = name,
            [nameof(Zone.Provider)] = provider,
            [nameof(Zone.Parameters)] = new Dictionary<string, string>(),
            [nameof(Zone.Subdomains)] = Array.Empty<string>()
        };
        
        if (save is true)
        {
            return new RazorComponentResult<Zone>(parameters);
        }

        return new RazorComponentResult<ZoneTemplate>(parameters);
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
