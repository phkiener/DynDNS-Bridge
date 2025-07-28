using DynDNS.Core;
using DynDNS.Providers;

namespace DynDNS.Application.Endpoints;

public static class ApplyBindingsEndpoint
{
    public static async Task Handle(HttpContext context)
    {
        var addressProvider = context.RequestServices.GetRequiredService<ICurrentAddressProvider>();
        await addressProvider.RefreshAsync(context.RequestAborted);
        
        var repository = context.RequestServices.GetRequiredService<IDomainRepository>();
        var clientProvider = context.RequestServices.GetRequiredService<ClientProvider>();

        foreach (var domainBinding in await repository.GetAllAsync())
        {
            var client = clientProvider.Client(domainBinding.Provider);
            await client.ApplyAsync(
                configuration: domainBinding.Configuration,
                subdomains: domainBinding.Subdomains.ToList(),
                targetAddressIPv4: domainBinding.BindIPv4 ? addressProvider.IPv4 : null,
                targetAddressIPv6: domainBinding.BindIPv6 ? addressProvider.IPv6 : null);
        }

        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
    }
}
