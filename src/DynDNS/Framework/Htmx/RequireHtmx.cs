using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

public sealed class RequireHtmxAttribute : ActionFilterAttribute
{
    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Headers.TryGetValue("hx-request", out var value) && value == "true")
        {
            return next();
        }

        context.Result = new NotFoundResult();
        return Task.CompletedTask;
    }
}
