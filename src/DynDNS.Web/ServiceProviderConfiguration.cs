using DynDNS.Web.Website.Overview;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace DynDNS.Web;

public static class ServiceProviderConfiguration
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddRazorComponents().AddInteractiveServerComponents();

        services.AddScoped<OverviewModel>();

        return services;
    }

}
