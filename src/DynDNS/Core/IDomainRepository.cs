using DynDNS.Core.Domains;

namespace DynDNS.Core;

public interface IDomainRepository
{
    Task<IReadOnlyList<DomainBinding>> GetAllAsync();
    Task DeleteAsync(DomainBinding domain);
}
