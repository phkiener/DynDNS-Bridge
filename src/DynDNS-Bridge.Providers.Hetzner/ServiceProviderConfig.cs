using DynDNS.Providers.Hetzner;
using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Providers;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddHetzner(this IServiceCollection services)
    {
        return services.AddProvider<HetznerProvider>();
    }
}
