using DynDNS.Framework;
using DynDNS.Framework.Htmx;
using Microsoft.AspNetCore.Components;

namespace DynDNS.WebInterface.Components;

[HtmxGet(RouteTemplate)]
public sealed partial class CurrentConnection(ICurrentAddressProvider currentAddress) : ComponentBase
{
    private const string RouteTemplate = "/_current-connection";
    
    [SupplyParameterFromQuery]
    public bool Refresh { get; set; } = false;
    
    [CascadingParameter]
    public required HttpContext HttpContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Refresh)
        {
            await currentAddress.RefreshAsync(HttpContext.RequestAborted);
        }
    }
}
