using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;

namespace DynDNS.Framework.Htmx;

public sealed class ComponentEndpointDataSource : EndpointDataSource
{
    private readonly Assembly[] assemblies;
    private readonly Lock lockObject = new();
    
    private List<Endpoint>? endpoints;
    private CancellationTokenSource endpontsChangedSource;
    private IChangeToken endpontsChangedToken;

    public ComponentEndpointDataSource(Assembly[] assemblies)
    {
        this.assemblies = assemblies;
        GenerateChangeToken();
    }
    
    public override IChangeToken GetChangeToken()
    {
        return endpontsChangedToken;
    }

    public override IReadOnlyList<Endpoint> Endpoints => endpoints ??= ExtractEndpoints().ToList();

    private List<Endpoint> ExtractEndpoints()
    {
        var foundEndpoints = new List<Endpoint>();
        lock (lockObject)
        {
            var components = assemblies.SelectMany(static a => a.GetExportedTypes())
                .Where(static t => t.GetCustomAttribute<HtmxComponentAttribute>() is not null)
                .ToList();

            foreach (var component in components)
            {
                var routes = component.GetCustomAttributes<HtmxComponentAttribute>();
                foreach (var route in routes)
                {
                    var endpointBuilder = new RouteEndpointBuilder(
                        requestDelegate: null,
                        routePattern: RoutePatternFactory.Parse(route.RouteTemplate),
                        order: 0);

                    endpointBuilder.Metadata.Add(new HttpMethodMetadata([route.HttpMethod.Method]));
                    endpointBuilder.Metadata.Add(new ComponentTypeMetadata(component));
                    endpointBuilder.Metadata.Add(new RootComponentMetadata(component));

                    foreach (var attribute in component.GetCustomAttributes())
                    {
                        if (attribute is HtmxComponentAttribute or RequiredMemberAttribute)
                        {
                            continue;
                        }
                        
                        endpointBuilder.Metadata.Add(attribute);
                    }
                    
                    endpointBuilder.RequestDelegate = static async httpContext =>
                    {
                        var isHtmx = httpContext.Request.Headers.TryGetValue("hx-request", out var value) && value.ToString().Equals("true",  StringComparison.OrdinalIgnoreCase);
                        if (!isHtmx)
                        {
                            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                            await httpContext.Response.CompleteAsync();
                        }
                        else
                        {
                            var invoker = httpContext.RequestServices.GetRequiredService<IRazorComponentEndpointInvoker>();
                            await invoker.Render(httpContext);
                        }
                    };

                    foundEndpoints.Add(endpointBuilder.Build());
                }
            }
        }
        
        GenerateChangeToken();
        return foundEndpoints;
    }

    [MemberNotNull(nameof(endpontsChangedSource))]
    [MemberNotNull(nameof(endpontsChangedToken))]
    private void GenerateChangeToken()
    {
        endpontsChangedSource = new CancellationTokenSource();
        endpontsChangedToken = new CancellationChangeToken(endpontsChangedSource.Token);
    }
}
