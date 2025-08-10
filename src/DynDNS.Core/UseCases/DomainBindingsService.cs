using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="plugins">The registered <see cref="IProviderPlugin"/>s.</param>
/// <param name="repository">The <see cref="IDomainBindingRepository"/> to use for persistence.</param>
public sealed class DomainBindingsService(
    IEnumerable<IProviderPlugin> plugins,
    IDomainBindingRepository repository) : IDomainBindings
{
    private readonly Dictionary<string, IProviderPlugin> plugins = plugins.ToDictionary(static p => p.Name);

    /// <inheritdoc />
    public async Task<DomainBindingId> CreateDomainBindingAsync(Hostname domain)
    {
        var existingBinding = await repository.GetByIdAsync(new DomainBindingId(domain));
        if (existingBinding is not null)
        {
            return existingBinding.Id;
        }

        var domainBinding = new Model.DomainBinding(domain);
        await repository.AddAsync(domainBinding);

        return domainBinding.Id;
    }

    /// <inheritdoc />
    public async Task RemoveDomainBindingAsync(DomainBindingId id)
    {
        var existingBinding = await repository.GetByIdAsync(id);
        if (existingBinding is null)
        {
            return;

        }

        if (existingBinding.ProviderConfiguration is not null)
        {
            var client = plugins[existingBinding.ProviderConfiguration.Name].GetClient(existingBinding.ProviderConfiguration.Parameters);
            foreach (var subdomain in existingBinding.Subdomains)
            {
                if (subdomain.CreateIPv4Record)
                {
                    await client.DeleteRecordAsync(existingBinding.Hostname, subdomain.Name, DnsRecordType.A);
                }

                if (subdomain.CreateIPv6Record)
                {
                    await client.DeleteRecordAsync(existingBinding.Hostname, subdomain.Name, DnsRecordType.AAAA);
                }
            }
        }

        await repository.RemoveAsync(existingBinding);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DomainBinding>> GetDomainBindingsAsync()
    {
        var bindings = await repository.GetAllAsync();

        return bindings
            .Select(MapBinding)
            .OrderBy(b => (string)b.Domain)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<DomainBinding?> FindDomainBindingAsync(Hostname hostname)
    {
        var existingBinding = await repository.GetByHostnameAsync(hostname);

        return existingBinding is null ? null : MapBinding(existingBinding);
    }

    private static DomainBinding MapBinding(Model.DomainBinding entity)
    {
        var subdomains = entity.Subdomains.Select(MapSubdomain).ToArray();
        var providerConfiugration = entity.ProviderConfiguration is null
            ? null
            : new DomainBinding.Provider(entity.ProviderConfiguration.Name, entity.ProviderConfiguration.Parameters);

        return new DomainBinding(entity.Id, entity.Hostname, subdomains, providerConfiugration);

        static DomainBinding.Subdomain MapSubdomain(Model.Subdomain entity)
        {
            var flags = SubdomainFlags.None;
            if (entity.CreateIPv4Record)
            {
                flags |= SubdomainFlags.A;
            }

            if (entity.CreateIPv6Record)
            {
                flags |= SubdomainFlags.AAAA;
            }

            return new DomainBinding.Subdomain(entity.Name, flags);
        }
    }
}
