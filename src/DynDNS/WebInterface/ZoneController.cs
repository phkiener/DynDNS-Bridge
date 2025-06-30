using DynDNS.Framework;
using DynDNS.Framework.Htmx;
using DynDNS.WebInterface.Components;
using DynDNS.Zones;
using Microsoft.AspNetCore.Mvc;

namespace DynDNS.WebInterface;

[RequireHtmx]
public sealed class ZoneController(IZoneRepository zoneRepository) : Controller
{
    [HttpGet("/interact/zones/_template")]
    public IActionResult AddZone()
    {
        var parameters = new Dictionary<string, object?>
        {
            [nameof(Zone.Name)] = "",
            [nameof(Zone.Provider)] = "",
            [nameof(Zone.Parameters)] = new Dictionary<string, string>(),
            [nameof(Zone.Subdomains)] = Array.Empty<string>()
        };

        return new RazorComponentActionResult<ZoneTemplate>(parameters);
    }
    
    [HttpPost("/interact/zones/_validate")]
    public IActionResult ValidateZone([FromForm] string? name, [FromForm] string? provider)
    {
        var parameters = new Dictionary<string, object?>
        {
            [nameof(Zone.Name)] = name ?? "",
            [nameof(Zone.Provider)] = provider ?? "",
            [nameof(Zone.Parameters)] = new Dictionary<string, string>(),
            [nameof(Zone.Subdomains)] = Array.Empty<string>()
        };

        return new RazorComponentActionResult<ZoneTemplate>(parameters);
    }
    
    [HttpPost("/interact/zones/")]
    public IActionResult AddZoneAsync()
    {
        var parameters = new Dictionary<string, object?>
        {
            [nameof(Zone.Name)] = "",
            [nameof(Zone.Provider)] = "",
            [nameof(Zone.Parameters)] = new Dictionary<string, string>(),
            [nameof(Zone.Subdomains)] = Array.Empty<string>()
        };

        return new RazorComponentActionResult<Zone>(parameters);
    }
    
    [HttpDelete("/interact/zones/{zone}")]
    public async Task<IActionResult> DeleteZoneAsync(string zone, CancellationToken cancellationToken)
    {
        await zoneRepository.DeleteZoneAsync(zone, cancellationToken);

        return Empty;
    }
}
