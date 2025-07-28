using DynDNS.Core.Domains;
using DynDNS.Core.Network;

namespace DynDNS.Core;

public static class ServiceProviderConfig
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IZoneRepository, ZoneRepository>();
        services.AddSingleton<IDomainRepository, InMemoryRepository>();
        
        services.AddSingleton<CurrentAddressProvider>();
        services.AddSingleton<ICurrentAddressProvider>(static sp => sp.GetRequiredService<CurrentAddressProvider>());
        services.AddHostedService(static sp => sp.GetRequiredService<CurrentAddressProvider>());
    }
}
