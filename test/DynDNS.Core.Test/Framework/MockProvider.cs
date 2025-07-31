using DynDNS.Core.Abstractions.Models;
using DynDNS.Provider.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DynDNS.Core.Test.Framework;

public sealed class MockProvider : IProviderPlugin
{
    public const string Name = "Mock";
    public const string ParameterName = "Parameter";

    string IProviderPlugin.Name => Name;

    IReadOnlyCollection<ProviderConfigurationParameter> IProviderPlugin.Parameters { get; } =
    [
        new(ParameterName, true, ParameterType.Text)
    ];
}

public static class MockProviderExtensions
{
    public static IServiceCollection UseMockProvider(this IServiceCollection services)
    {
        services.RemoveAll<IProviderPlugin>();
        services.AddSingleton<IProviderPlugin, MockProvider>();

        return services;
    }
}
