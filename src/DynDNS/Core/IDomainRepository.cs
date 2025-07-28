using DynDNS.Core.Domains;

namespace DynDNS.Core;

public interface IDomainRepository
{
    Task<IReadOnlyList<DomainBinding>> GetAllAsync();
    Task<DomainBinding?> FindAsync(string domain);
    Task DeleteAsync(DomainBinding domain);
    Task Add(DomainBinding domain);
}
