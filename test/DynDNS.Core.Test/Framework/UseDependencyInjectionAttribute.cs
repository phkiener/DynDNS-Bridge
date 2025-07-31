using Microsoft.Extensions.DependencyInjection;

namespace DynDNS.Core.Test.Framework;

public sealed class UseDependencyInjectionAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider SharedServiceProvider = new ServiceCollection()
        .AddCoreServices()
        .UseTransientCore(useScopes: true)
        .UseMockProvider()
        .BuildServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return SharedServiceProvider.CreateScope();
    }

    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }
}
