using DynDNS.Core.Abstractions;
using DynDNS.Core.Infrastructure;
using DynDNS.Core.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DynDNS.Core;

/// <summary>
/// Extensions on a <see cref="IServiceCollection"/> to register required interfaces.
/// </summary>
public static class ServiceProviderConfiguration
{
    /// <summary>
    /// Register the required services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainBindings, DomainBindingsService>();
        services.AddScoped<ISubdomains, SubdomainsService>();
        services.AddScoped<IProviderConfigurations, ProviderConfigurationsService>();

        return services;
    }

    /// <summary>
    /// Replace all registrations with a "dry-run"-like mode; persistence is in memory and no outgoing HTTP calls will be made to
    /// any provider.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> in which to replace the services.</param>
    /// <param name="useScopes">Whether to use singleton registrations (by default) or scoped registrations (for tests).</param>
    public static IServiceCollection UseTransientCore(this IServiceCollection services, bool useScopes = false)
    {
        services.RemoveAll<IDomainBindingRepository>();
        if (useScopes)
        {
            services.AddScoped<IDomainBindingRepository, InMemoryRepository>();
        }
        else
        {
            services.AddSingleton<IDomainBindingRepository, InMemoryRepository>();
        }

        return services;
    }
}
