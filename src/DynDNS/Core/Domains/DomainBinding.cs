using DynDNS.Providers;

namespace DynDNS.Core.Domains;

public sealed class DomainBinding
{
    public string Id => Domain;
    public string Domain { get; }
    public string Provider => Configuration.Provider;

    public bool BindIPv4 { get; private set; }
    public bool BindIPv6 { get; private set; }

    private readonly HashSet<string> subdomains = [];
    public IEnumerable<string> Subdomains => subdomains.AsEnumerable();
    
    public DomainBindingConfiguration Configuration { get; }

    private DomainBinding(string domain, bool bindIPv4, bool bindIPv6, DomainBindingConfiguration configuration)
    {
        Domain = domain;
        BindIPv4 = bindIPv4;
        BindIPv6 = bindIPv6;
        Configuration = configuration;
    }

    public static DomainBinding Create(string domain, DomainBindingConfiguration configuration)
    {
        return new DomainBinding(domain, false, false, configuration);
    }

    public void ToggleBindIPv4(bool enabled)
    {
        BindIPv4 = enabled;
    }

    public void ToggleBindIPv6(bool enabled)
    {
        BindIPv6 = enabled;
    }

    public void AddSubdomain(string subdomain)
    {
        subdomains.Add(subdomain);
    }

    public void RemoveSubdomain(string subdomain)
    {
        subdomains.Remove(subdomain);
    }

    public static DomainBinding Create(string domain, bool bindIPv4, bool bindIPv6, DomainBindingConfiguration configuration, IEnumerable<string> subdomains)
    {
        var binding = new DomainBinding(domain, bindIPv4, bindIPv6, configuration);
        
        foreach (var subdomain in subdomains)
        {
            binding.subdomains.Add(subdomain);
        }

        return binding;
    }
}
