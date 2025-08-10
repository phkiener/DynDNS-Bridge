using DynDNS.Web.Api;
using DynDNS.Web.Website;
using Microsoft.AspNetCore.Builder;

namespace DynDNS.Web;

public static class WebApplicationConfiguration
{
    public static void UseWebsite(this WebApplication application)
    {
        application.MapStaticAssets();
        application.UseStatusCodePagesWithReExecute("/status/{0}");
        application.MapGet("api/v1/refresh", RefreshEntriesEndpoint.HandleAsync);
        application.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .DisableAntiforgery();
    }
}
