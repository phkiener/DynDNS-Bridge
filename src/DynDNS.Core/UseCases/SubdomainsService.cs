using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="repository">The <see cref="IDomainBindingRepository"/> to use for persistence</param>
public sealed class SubdomainsService(IDomainBindingRepository repository) : ISubdomains
{
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
        
        domainBinding.RemoveSubdomain(name);
        await repository.UpdateAsync(domainBinding);
    }
}
