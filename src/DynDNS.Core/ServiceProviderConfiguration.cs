using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Core;

public static class ServiceProviderConfiguration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection UseTransientCore(this IServiceCollection services)
    {
        return services;
    }
}
