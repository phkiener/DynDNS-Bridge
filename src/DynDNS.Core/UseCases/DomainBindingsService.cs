using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="repository">The <see cref="IDomainBindingRepository"/> to use for persistence.</param>
public sealed class DomainBindingsService(IDomainBindingRepository repository) : IDomainBindings
{
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
        if (existingBinding is not null)
        {
            await repository.RemoveAsync(existingBinding);
        }
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

        return new DomainBinding(entity.Id, entity.Hostname, subdomains);

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
