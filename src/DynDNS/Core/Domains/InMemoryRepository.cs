using DynDNS.Providers.Hetzner;

namespace DynDNS.Core.Domains;

public sealed class InMemoryRepository : IDomainRepository
{
    private readonly List<DomainBinding> domainBindings =
    [
        DomainBinding.Create(
            domain: "phkiener.ch",
            bindIPv4: true,
            bindIPv6: false,
            configuration: new HetznerBindingConfiguration("AGW24N", "foobar"),
            subdomains: ["jellyfin", "dns", "vpn"]),
        DomainBinding.Create(
            domain: "example.ch",
            bindIPv4: false,
            bindIPv6: false,
            configuration: new HetznerBindingConfiguration("JANS1", "barquuz"),
            subdomains: ["blog"]),
        DomainBinding.Create(
            domain: "another-one.ch",
            bindIPv4: false,
            bindIPv6: false,
            configuration: new HetznerBindingConfiguration("LUHN12", "quuzfoo"),
            subdomains: ["www", "public", "more.levels"]),
    ];
    
    public Task<IReadOnlyList<DomainBinding>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<DomainBinding>>(domainBindings);
    }

    public Task DeleteAsync(DomainBinding domain)
    {
        domainBindings.Remove(domain);
        return Task.CompletedTask;
    }
}
