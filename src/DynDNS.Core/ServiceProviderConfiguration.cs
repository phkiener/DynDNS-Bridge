using DynDNS.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Core;

public static class ServiceProviderConfiguration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainBindings, DomainBindingsService>();

        return services;
    }

    public static IServiceCollection UseTransientCore(this IServiceCollection services)
    {
        return services;
    }
}
