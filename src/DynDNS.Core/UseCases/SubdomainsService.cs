using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="plugins">The registered <see cref="IProviderPlugin"/>s.</param>
/// <param name="repository">The <see cref="IDomainBindingRepository"/> to use for persistence</param>
public sealed class SubdomainsService(
    IEnumerable<IProviderPlugin> plugins,
    IDomainBindingRepository repository) : ISubdomains
{
    private readonly Dictionary<string, IProviderPlugin> plugins = plugins.ToDictionary(static p => p.Name);

    public async Task AddSubdomainAsync(DomainBindingId id, DomainFragment name)
    {
        var domainBinding = await repository.GetByIdAsync(id);
        if (domainBinding is null)
        {
            throw new InvalidOperationException($"DomainBinding {id} does not exist.");
        }

        domainBinding.AddSubdomain(name);
        await repository.UpdateAsync(domainBinding);
    }

    public async Task UpdateSubdomainAsync(DomainBindingId id, DomainFragment name, SubdomainFlags flags)
    {
        var domainBinding = await repository.GetByIdAsync(id);
        if (domainBinding is null)
        {
            throw new InvalidOperationException($"DomainBinding {id} does not exist.");
        }

        var subdomain = domainBinding.FindSubdomain(name);
        if (subdomain is null)
        {
            throw new InvalidOperationException($"Subdomain {name} is not configured for {domainBinding.Hostname}");
        }

        subdomain.ApplyFlags(flags);
        await repository.UpdateAsync(domainBinding);
    }

    public async Task RemoveSubdomainAsync(DomainBindingId id, DomainFragment name)
    {
        var domainBinding = await repository.GetByIdAsync(id);
        if (domainBinding is null)
        {
            throw new InvalidOperationException($"DomainBinding {id} does not exist.");
        }

        var subdomain = domainBinding.FindSubdomain(name);
        if (subdomain is null)
        {
            return;
        }

        if (domainBinding.ProviderConfiguration is not null)
        {
            var client = plugins[domainBinding.ProviderConfiguration.Name].GetClient(domainBinding.ProviderConfiguration.Parameters);
            if (subdomain.CreateIPv4Record)
            {
                await client.DeleteRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.A);
            }

            if (subdomain.CreateIPv6Record)
            {
                await client.DeleteRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.AAAA);
            }
        }

        domainBinding.RemoveSubdomain(name);
        await repository.UpdateAsync(domainBinding);
    }
}
