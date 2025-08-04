using DynDNS.Core.Abstractions.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Core.Plugins.Hetzner;

/// <summary>
/// Extensions on a <see cref="IServiceCollection"/> to register required the Hetzner provider.
/// </summary>
public static class ServiceProviderConfiguration
{
    /// <summary>
    /// Register the required services for Hetzner.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
    public static IServiceCollection AddHetznerPlugin(this IServiceCollection services)
    {
        services.AddSingleton<IProviderPlugin, HetznerPlugin>();
        return services;
    }
}
