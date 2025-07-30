using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core;

public sealed class DomainBindingsService : IDomainBindings
{
    public Task<DomainBindingId> CreateDomainBindingAsync(Hostname domain, string provider)
    {
        throw new NotImplementedException();
    }

    public Task RemoveDomainBindingAsync(DomainBindingId id)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyCollection<DomainBinding>> GetDomainBindingsAsync()
    {
        return [];
    }
}
