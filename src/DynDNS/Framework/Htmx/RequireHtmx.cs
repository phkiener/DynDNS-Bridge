namespace DynDNS.Framework.Htmx;

public sealed class RequireHtmx : IEndpointFilter
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