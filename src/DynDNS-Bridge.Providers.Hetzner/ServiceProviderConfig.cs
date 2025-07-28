using DynDNS.Providers.Hetzner;
using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Providers;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddHetzner(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(HetznerClient), static c =>
        {
            c.BaseAddress = new Uri("https://dns.hetzner.com/api/v1/");
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services.AddProvider<HetznerProvider, HetznerClient>();
    }
}
