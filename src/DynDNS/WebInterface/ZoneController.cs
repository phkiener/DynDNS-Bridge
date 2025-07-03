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
            [nameof(Zone.BindIPv4)] = false,
            [nameof(Zone.BindIPv6)] = false,
            [nameof(Zone.ApiKey)] = "",
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
            [nameof(Zone.BindIPv4)] = false,
            [nameof(Zone.BindIPv6)] = false,
            [nameof(Zone.ApiKey)] = "",
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
            [nameof(Zone.BindIPv4)] = false,
            [nameof(Zone.BindIPv6)] = false,
            [nameof(Zone.ApiKey)] = "",
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
    
    [HttpDelete("/interact/zones/{zone}/subdomains/{subdomain}")]
    public async Task<IActionResult> DeleteSubdomainAsync(string zone, string subdomain, CancellationToken cancellationToken)
    {
        await zoneRepository.UpdateZoneAsync(zone, z => z with { Subdomains = z.Subdomains.Except([subdomain]).ToList() }, cancellationToken);

        return Empty;
    }
}
