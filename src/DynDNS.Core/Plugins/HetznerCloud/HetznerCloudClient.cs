using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;
using Microsoft.Extensions.Logging;

namespace DynDNS.Core.Plugins.HetznerCloud;

internal sealed class HetznerCloudClient(HttpClient client, string label, ILogger logger) : IProviderClient
{
    public async Task CreateRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type, string ipAddress)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type)
    {
        throw new NotImplementedException();
    }
}
