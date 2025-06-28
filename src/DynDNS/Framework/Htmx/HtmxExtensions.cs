using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DynDNS.Framework.Htmx;

public static class HtmxExtensions
{
    public static IEndpointConventionBuilder MapHtmxPost<T>(
        this IEndpointRouteBuilder endpoint,
        [StringSyntax("Route")] string path,
        Func<HttpContext, Task> action) where T : IComponent
    {
        return endpoint.MapPost(path, InvokeAndRender<T>(action)).AddEndpointFilter<RequireHtmx>();
    }

    private static Delegate InvokeAndRender<T>(Func<HttpContext, Task> action) where T : IComponent
    {
        return async (HttpContext ctx) =>
        {
            await action(ctx);
            return new RazorComponentResult<T>();
        };
    }
    
    private sealed class RequireHtmx : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("hx-request", out var value) && value == "true")
            {
                return await next(context);
            }

            return Results.NotFound();
        }
    }

}
