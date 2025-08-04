using DynDNS.Core.Abstractions.Models;
using DomainBinding = DynDNS.Core.Model.DomainBinding;

namespace DynDNS.Core.Transient;

internal sealed class InMemoryRepository : IDomainBindingRepository
{
    private readonly List<DomainBinding> bindings = [];

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
