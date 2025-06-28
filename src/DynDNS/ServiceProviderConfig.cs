using DynDNS.Framework;
using DynDNS.Framework.Htmx;
using DynDNS.Framework.Network;
using DynDNS.WebInterface;
using DynDNS.Zones;

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

        builder.Services.AddSingleton<IZoneRepository, ZoneRepository>();
    }

    public static void ConfigureEndpoints(this WebApplication app)
    {
        app.MapStaticAssets();
        app.UseAntiforgery();

        app.MapRazorComponents<Root>();
        app.MapPost("interact/refresh-address", Endpoints.RefreshAddress).AddEndpointFilter<RequireHtmx>();
        app.MapDelete("interact/delete-zone/{name}", Endpoints.DeleteZone).AddEndpointFilter<RequireHtmx>();
    }
}
