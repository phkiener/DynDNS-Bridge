using DynDNS.Core.Abstractions.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Core.Plugins.HetznerCloud;

/// <summary>
/// Extensions on a <see cref="IServiceCollection"/> to register required the Hetzner Cloud provider.
/// </summary>
public static class ServiceProviderConfiguration
{
    /// <summary>
    /// Register the required services for Hetzner Cloud.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
    public static IServiceCollection AddHetznerCloudPlugin(this IServiceCollection services)
    {
        services.AddSingleton<IProviderPlugin, HetznerCloudPlugin>();
        services.AddHttpClient(nameof(HetznerCloudClient), static c => c.BaseAddress = new Uri("https://api.hetzner.cloud/v1/"));

        return services;
    }
}
