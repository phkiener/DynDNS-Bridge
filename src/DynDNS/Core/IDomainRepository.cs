using DynDNS.Core.Domains;

namespace DynDNS.Core;

public interface IDomainRepository
{
    Task<IReadOnlyList<DomainBinding>> GetAllAsync();
    Task<DomainBinding?> FindAsync(string domain);

    Task AddAsync(DomainBinding domain);
    Task UpdateAsync(DomainBinding domain);
    Task DeleteAsync(DomainBinding domain);
}
