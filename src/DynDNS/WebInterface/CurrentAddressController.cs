using DynDNS.Framework;
using DynDNS.Framework.Htmx;
using DynDNS.WebInterface.Components;
using Microsoft.AspNetCore.Mvc;

namespace DynDNS.WebInterface;

[RequireHtmx]
public sealed class CurrentAddressController(ICurrentAddressProvider currentAddressProvider) : Controller
{
    [HttpPost("interact/_refresh-address")]
    public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken)
    {
        await currentAddressProvider.RefreshAsync(cancellationToken);

        return new RazorComponentActionResult<CurrentConnection>();
    }
}
