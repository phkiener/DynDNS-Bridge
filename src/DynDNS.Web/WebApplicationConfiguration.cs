using DynDNS.Web.Website;
using Microsoft.AspNetCore.Builder;

namespace DynDNS.Web;

public static class WebApplicationConfiguration
{
    public static void UseWebsite(this WebApplication application)
    {
        application.MapStaticAssets();
        application.UseStatusCodePagesWithReExecute("/status/{0}");
        application.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .DisableAntiforgery();
    }
}
