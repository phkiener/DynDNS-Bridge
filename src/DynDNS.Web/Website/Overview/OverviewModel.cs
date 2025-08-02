using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Web.Website.Overview;

public sealed class DomainBindingModel
{
    public required DomainBindingId Id { get; init; }

    public string Hostname { get; set; } = "";
}

public sealed class SubdomainModel
{
    public required DomainBindingId DomainBindingId { get; init; }

    public string Name { get; set; } = "";
    public bool BindIPv4 { get; set; } = false;
    public bool BindIPv6 { get; set; } = false;
}

public sealed class OverviewModel(IDomainBindings domainBindings, ISubdomains subdomains)
{
    private bool isLoaded = false;

    private readonly List<DomainBindingModel> bindingModels = [];
    public IReadOnlyList<DomainBindingModel> DomainBindings => bindingModels.AsReadOnly();
    public event EventHandler<DomainBindingModel>? OnDomainBindingAdded;
    public event EventHandler<DomainBindingModel>? OnDomainBindingRemoved;

    private readonly List<SubdomainModel> subdomainModels = [];
    public IReadOnlyList<SubdomainModel> Subdomains => subdomainModels.AsReadOnly();
    public event EventHandler<SubdomainModel>? OnSubdomainAdded;
    public event EventHandler<SubdomainModel>? OnSubdomainRemoved;

    public async Task InitializeAsync()
    {
        if (!isLoaded)
        {
            foreach (var domainBinding in await domainBindings.GetDomainBindingsAsync())
            {
                var model = new DomainBindingModel
                {
                    Id = domainBinding.Id,
                    Hostname = domainBinding.Domain
                };

                bindingModels.Add(model);

                foreach (var subdomain in domainBinding.Subdomains)
                {
                    var subdomainModel = new SubdomainModel
                    {
                        DomainBindingId = domainBinding.Id,
                        Name = subdomain.Name,
                        BindIPv4 = subdomain.Flags.HasFlag(SubdomainFlags.A),
                        BindIPv6 = subdomain.Flags.HasFlag(SubdomainFlags.AAAA),
                    };

                    subdomainModels.Add(subdomainModel);
                }
            }

            isLoaded = true;
        }
    }

    public async Task AddSubdomain(DomainBindingModel domainBinding, string subdomain)
    {
        await subdomains.AddSubdomainAsync(domainBinding.Id, DomainFragment.From(subdomain));

        var subdomainModel = new SubdomainModel { DomainBindingId = domainBinding.Id, Name = subdomain };
        subdomainModels.Add(subdomainModel);

        OnSubdomainAdded?.Invoke(this, subdomainModel);
    }

    public async Task UpdateSubdomain(SubdomainModel subdomain)
    {
        var flags = SubdomainFlags.None;
        flags |= subdomain.BindIPv4 ? SubdomainFlags.A : SubdomainFlags.None;
        flags |= subdomain.BindIPv6 ? SubdomainFlags.AAAA : SubdomainFlags.None;

        await subdomains.UpdateSubdomainAsync(subdomain.DomainBindingId, DomainFragment.From(subdomain.Name), flags);
    }

    public async Task RemoveSubdomain(SubdomainModel subdomain)
    {
        await subdomains.RemoveSubdomainAsync(subdomain.DomainBindingId, DomainFragment.From(subdomain.Name));
        subdomainModels.Remove(subdomain);

        OnSubdomainRemoved?.Invoke(this, subdomain);
    }

    public async Task AddDomainBinding(string hostname)
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From(hostname));

        var model = new DomainBindingModel { Id = id, Hostname = hostname };
        bindingModels.Add(model);

        OnDomainBindingAdded?.Invoke(this, model);
    }

    public async Task DeleteDomainBinding(DomainBindingModel domainBinding)
    {
        await domainBindings.RemoveDomainBindingAsync(domainBinding.Id);
        bindingModels.Remove(domainBinding);

        OnDomainBindingRemoved?.Invoke(this, domainBinding);
    }
}
