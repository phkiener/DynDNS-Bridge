using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using Swallow.Validation;
using Swallow.Validation.Assertions;

namespace DynDNS.Web.Website.Overview;

public sealed class DomainBindingModel : IValidatable
{
    public DomainBindingId Id { get; init; } = default;

    public string Domain { get; set; } = "";

    public ValidationResult Validate()
    {
        return Validator.Check()
            .That(Domain).IsNotEmpty().Satisfies(static s => Hostname.TryCreate(s, out _), "be a valid hostname")
            .Result();
    }
}

public sealed class SubdomainModel : IValidatable
{
    public required DomainBindingId DomainBindingId { get; init; }

    public string Name { get; set; } = "";
    public bool BindIPv4 { get; set; } = false;
    public bool BindIPv6 { get; set; } = false;

    public ValidationResult Validate()
    {
        return Validator.Check()
            .That(Name).IsNotEmpty().Satisfies(static s => DomainFragment.TryCreate(s, out _), "be a valid domain fragment")
            .Result();
    }
}

public sealed class ProviderConfigurationModel
{
    public required DomainBindingId DomainBindingId { get; init; }

    public string? Provider { get; set; } = null;
    public IReadOnlyList<string> RequiredParameters { get; set; } = [];
    public IDictionary<string, string> SuppliedParameters { get; set; } = new Dictionary<string, string>();
}

public sealed class OverviewModel(IDomainBindings domainBindings, ISubdomains subdomains, IProviderConfigurations providerConfigurations)
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

    private readonly List<ProviderConfigurationModel> providerConfigurationModels = [];
    public IReadOnlyList<ProviderConfigurationModel> ProviderConfigurations => providerConfigurationModels.AsReadOnly();
    public event EventHandler<ProviderConfigurationModel>? OnProviderConfigurationChanged;

    public IReadOnlyList<string> AvailableProviders => providerConfigurations.Providers.Order().ToList();

    public async Task InitializeAsync()
    {
        if (!isLoaded)
        {
            foreach (var domainBinding in await domainBindings.GetDomainBindingsAsync())
            {
                var model = new DomainBindingModel { Id = domainBinding.Id, Domain = domainBinding.Domain };
                bindingModels.Add(model);

                var providerConfigurationModel = new ProviderConfigurationModel { DomainBindingId = domainBinding.Id };
                if (domainBinding.ConfiguredProvider is not null)
                {
                    providerConfigurationModel.Provider = domainBinding.ConfiguredProvider.Name;
                    providerConfigurationModel.RequiredParameters = providerConfigurations.GetParameters(domainBinding.ConfiguredProvider.Name);
                    providerConfigurationModel.SuppliedParameters = new Dictionary<string, string>(domainBinding.ConfiguredProvider.Parameters);
                }

                providerConfigurationModels.Add(providerConfigurationModel);

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

    public async Task AddSubdomain(SubdomainModel subdomain)
    {
        await subdomains.AddSubdomainAsync(subdomain.DomainBindingId, DomainFragment.From(subdomain.Name));
        subdomainModels.Add(subdomain);

        OnSubdomainAdded?.Invoke(this, subdomain);
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

    public async Task AddDomainBinding(DomainBindingModel model)
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From(model.Domain));
        model = new DomainBindingModel { Id = id, Domain = model.Domain };
        bindingModels.Add(model);

        var providerConfigurationModel = new ProviderConfigurationModel { DomainBindingId = model.Id };
        providerConfigurationModels.Add(providerConfigurationModel);

        OnDomainBindingAdded?.Invoke(this, model);
    }

    public async Task DeleteDomainBinding(DomainBindingModel domainBinding)
    {
        await domainBindings.RemoveDomainBindingAsync(domainBinding.Id);
        bindingModels.Remove(domainBinding);

        OnDomainBindingRemoved?.Invoke(this, domainBinding);
    }

    public Task UpdateForProvider(ProviderConfigurationModel providerConfiguration)
    {
        if (providerConfiguration.Provider is null)
        {
            providerConfiguration.RequiredParameters = [];
            providerConfiguration.SuppliedParameters = new Dictionary<string, string>();
        }
        else
        {
            providerConfiguration.RequiredParameters = providerConfigurations.GetParameters(providerConfiguration.Provider);
            providerConfiguration.SuppliedParameters = providerConfiguration.RequiredParameters.ToDictionary(
                static key => key,
                key => providerConfiguration.SuppliedParameters.TryGetValue(key, out var parameter) ? parameter : "");
        }

        return Task.CompletedTask;
    }

    public async Task UpdateProviderConfiguration(ProviderConfigurationModel providerConfiguration)
    {
        if (providerConfiguration.Provider is null)
        {
            return; // We don't support deleting yet.
        }

        await providerConfigurations.ConfigureProviderAsync(
            id: providerConfiguration.DomainBindingId,
            provider: providerConfiguration.Provider,
            parameters: providerConfiguration.SuppliedParameters.AsReadOnly());

        var existingConfigurationIndex = providerConfigurationModels.FindIndex(c => c.DomainBindingId == providerConfiguration.DomainBindingId);
        if (existingConfigurationIndex is -1)
        {
            providerConfigurationModels.Add(providerConfiguration);
            OnProviderConfigurationChanged?.Invoke(this, providerConfiguration);
        }
        else
        {
            providerConfigurationModels[existingConfigurationIndex] = providerConfiguration;
            OnProviderConfigurationChanged?.Invoke(this, providerConfiguration);
        }
    }

    public async Task ApplyBinding(DomainBindingModel domainBinding)
    {
        await providerConfigurations.UpdateBindingsAsync(domainBinding.Id);
    }
}
