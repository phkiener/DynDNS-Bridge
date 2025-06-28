using DynDNS.Framework;
using DynDNS.Framework.Htmx;
using DynDNS.Framework.Network;
using DynDNS.WebInterface;
using DynDNS.WebInterface.Components;

namespace DynDNS;

public static class ServiceProviderConfig
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.user.json",  optional: true, reloadOnChange: true);
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents();
        builder.Services.AddHttpClient();
        
        builder.Services.AddSingleton<CurrentAddressProvider>();
        builder.Services.AddSingleton<ICurrentAddressProvider>(static sp => sp.GetRequiredService<CurrentAddressProvider>());
        builder.Services.AddHostedService(static sp => sp.GetRequiredService<CurrentAddressProvider>());
    }

    public static void ConfigureEndpoints(this WebApplication app)
    {
        app.MapStaticAssets();
        app.UseAntiforgery();

        app.MapRazorComponents<Root>();
        app.MapHtmxPost<CurrentConnection>("interact/refresh-address", Invoke.On<ICurrentAddressProvider>(static (r, ct) => r.RefreshAsync(ct)));
    }
}
