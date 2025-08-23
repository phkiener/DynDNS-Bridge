using DynDNS.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynDNS.Web.Api;

public static class RefreshEntriesEndpoint
{
    public static async Task HandleAsync(HttpContext context, [FromServices] IProviderConfigurations providerConfigurations)
    {
        await providerConfigurations.UpdateAllBindingsAsync(context.RequestAborted);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.CompleteAsync();
    }
}
