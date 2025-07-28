using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Providers;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddDomainProviders(this IServiceCollection services)
    {
        return services.AddSingleton<ConfigurationProvider>();
    }
    
    public static IServiceCollection AddProvider<TProvider, TClient>(this IServiceCollection services)
        where TProvider : class, IProvider
        where TClient : class, IProviderClient
    {
        services.AddSingleton<IProvider, TProvider>();
        services.AddScoped<IProviderClient, TClient>();

        return services;
    }
}
