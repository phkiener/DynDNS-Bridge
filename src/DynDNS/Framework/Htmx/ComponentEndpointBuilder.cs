using System.Reflection;

namespace DynDNS.Framework.Htmx;

public static class ComponentEndpointBuilder
{
    public static void MapHtmxComponents(this IEndpointRouteBuilder builder, params Assembly[] assemblies)
    {
        var dataSource = builder.DataSources.OfType<ComponentEndpointDataSource>().FirstOrDefault();
        if (dataSource is null)
        {
            dataSource = new ComponentEndpointDataSource(assemblies);
            builder.DataSources.Add(dataSource);
        }
    }
}
