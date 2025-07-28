using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Providers;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddDomainProviders(this IServiceCollection services)
    {
        services.AddSingleton<ConfigurationProvider>();
        services.AddScoped<ClientProvider>();
        
        return services;
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
