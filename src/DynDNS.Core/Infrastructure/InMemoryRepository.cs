using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Model;
using DomainBinding = DynDNS.Core.Model.DomainBinding;

namespace DynDNS.Core.Infrastructure;

internal sealed class InMemoryRepository : IDomainBindingRepository
{
    private readonly List<DomainBinding> bindings =
    [
        new(
            id: new DomainBindingId("phkiener.ch"),
            hostname: Hostname.From("phkiener.ch"),
            subdomains:
            [
                new Subdomain(DomainFragment.From("vpn")),
                new Subdomain(DomainFragment.From("jellyfin"))
            ])
    ];

    public Task AddAsync(DomainBinding domainBinding)
    {
        bindings.Add(domainBinding);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(DomainBinding domainBinding)
    {
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<DomainBinding>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyCollection<DomainBinding>>(bindings);
    }

    public Task<DomainBinding?> GetByIdAsync(DomainBindingId id)
    {
        var binding = bindings.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(binding);
    }

    public Task<DomainBinding?> GetByHostnameAsync(Hostname hostname)
    {
        var binding = bindings.FirstOrDefault(b => b.Hostname == hostname);
        return Task.FromResult(binding);
    }

    public Task RemoveAsync(DomainBinding domainBinding)
    {
        bindings.Remove(domainBinding);

        return Task.CompletedTask;
    }
}
