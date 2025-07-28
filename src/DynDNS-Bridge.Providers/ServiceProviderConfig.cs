using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Providers;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddDomainProviders(this IServiceCollection services)
    {
        return services.AddSingleton<ConfigurationProvider>();
    }
    
    public static IServiceCollection AddProvider<TProvider>(this IServiceCollection services) where TProvider : class, IProvider
    {
        return services.AddSingleton<IProvider, TProvider>();
    }
}
