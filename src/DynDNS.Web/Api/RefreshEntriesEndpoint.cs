using DynDNS.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynDNS.Web.Api;

public static class RefreshEntriesEndpoint
{
    public static async Task HandleAsync(
        HttpContext context,
        [FromServices] IDomainBindings domainBindings,
        [FromServices] ICurrentAddress currentAddress,
        [FromServices] IProviderConfigurations providerConfigurations)
    {
        await currentAddress.RefreshAsync();

        var bindings = await domainBindings.GetDomainBindingsAsync();
        foreach (var binding in bindings.Where(static b => b.ConfiguredProvider is not null))
        {
            await providerConfigurations.UpdateBindingsAsync(binding.Id);
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.CompleteAsync();
    }
}
