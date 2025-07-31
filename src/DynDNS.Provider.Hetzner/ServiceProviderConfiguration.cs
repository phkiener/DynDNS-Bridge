using DynDNS.Provider.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Provider.Hetzner;

/// <summary>
/// Extensions on a <see cref="IServiceCollection"/> to register the Hetzner plugin.
/// </summary>
public static class ServiceProviderConfiguration
{
    /// <summary>
    /// Register the required services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
    public static IServiceCollection AddHetzner(this IServiceCollection services)
    {
        services.AddSingleton<IProviderPlugin, HetznerProviderPlugin>();
        return services;
    }
}
