using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DynDNS.Framework;

public static class HtmxExtensions
{
    public static IEndpointConventionBuilder MapHtmxPost<T>(
        this IEndpointRouteBuilder endpoint,
        [StringSyntax("Route")] string path,
        Func<HttpContext, Task> action) where T : IComponent
    {
        return endpoint.MapPost(path, InvokeAndRender<T>(action)).AddEndpointFilter<HtmxOnlyFilter>();
    }

    private static Delegate InvokeAndRender<T>(Func<HttpContext, Task> action) where T : IComponent
    {
        return async (HttpContext ctx) =>
        {
            await action(ctx);
            return new RazorComponentResult<T>();
        };
    }
}
