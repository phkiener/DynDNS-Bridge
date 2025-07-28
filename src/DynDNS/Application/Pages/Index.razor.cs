using DynDNS.Application.Components;
using DynDNS.Core;
using DynDNS.Core.Domains;
using DynDNS.Providers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ConfigurationProvider = DynDNS.Providers.ConfigurationProvider;

namespace DynDNS.Application.Pages;

public sealed partial class Index(
    ConfigurationProvider configurationProvider,
    ClientProvider clientProvider,
    IDomainRepository domainRepository,
    IDialogService dialogService,
    ISnackbar snackbar) : ComponentBase
{
    private bool isAdding = false;
    private List<DomainBindingModel> Domains { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Domains = (await domainRepository.GetAllAsync()).Select(Map).ToList();
    }

    private static DomainBindingModel Map(DomainBinding entity)
    {
        return new DomainBindingModel
        {
            Provider = entity.Provider,
            Domain = entity.Domain,
            BindIPv4 = entity.BindIPv4,
            BindIPv6 = entity.BindIPv6,
            Subdomains = entity.Subdomains.ToList(),
            Parameters = entity.Configuration.Snapshot().ToDictionary()
        };
    }

    private async Task OnDelete(DomainBindingModel model)
    {
        var domainBinding = await domainRepository.FindAsync(model.Domain);
        if (domainBinding is null)
        {
            Domains.Remove(model);
            StateHasChanged();

            return;
        }

        var deletionConfirmed = await dialogService.ShowMessageBox(
            title: $"Deleting zone {domainBinding.Id}",
            message: "Are you sure you want to delete this domain?",
            yesText: "Yes",
            noText: "No");

        if (deletionConfirmed is true)
        {
            await domainRepository.DeleteAsync(domainBinding);

            Domains.Remove(model);
            StateHasChanged();
        }
    }

    private async Task AddSubdomain(DomainBindingModel model, string subdomain)
    {
        var domainBinding = await domainRepository.FindAsync(model.Domain);
        if (domainBinding is null)
        {
            return;
        }
        
        domainBinding.AddSubdomain(subdomain);
        await domainRepository.UpdateAsync(domainBinding);
        
        model.Subdomains.Insert(0, subdomain);
        StateHasChanged();
    }

    private async Task RemoveSubdomain(DomainBindingModel model, string subdomain)
    {
        var domainBinding = await domainRepository.FindAsync(model.Domain);
        if (domainBinding is null)
        {
            return;
        }
        
        domainBinding.RemoveSubdomain(subdomain);
        await domainRepository.UpdateAsync(domainBinding);
        
        model.Subdomains.Remove(subdomain);
        StateHasChanged();
    }

    private async Task EditConfig(DomainBindingModel model, string key, string value)
    {
        var domainBinding = await domainRepository.FindAsync(model.Domain);
        if (domainBinding is null)
        {
            return;
        }

        domainBinding.ChangeConfiguration(key, value);
        await domainRepository.UpdateAsync(domainBinding);
        
        model.Parameters[key] = value;
        StateHasChanged();
    }

    private async Task ApplyBinding(DomainBindingModel model)
    {
        var domainBinding = await domainRepository.FindAsync(model.Domain);
        if (domainBinding is null)
        {
            return;
        }

        var client = clientProvider.Client(domainBinding.Provider);
        await client.ApplyAsync(domainBinding.Configuration);

        snackbar.Add(
            $"Binding for {model.Domain} applied",
            Severity.Success,
            static opt =>
            {
                opt.ShowTransitionDuration = 250;
                opt.HideTransitionDuration = 250;
                opt.VisibleStateDuration = 1500;
            });
    }

    private void BeginAdd()
    {
        isAdding = true;
    }

    private void EndAdd()
    {
        isAdding = false;
    }

    private async Task CreateDomainBinding(CreateDomainBindingModel model)
    {
        var configuration = configurationProvider.Default(model.Provider);
        configuration.Apply(model.Parameters);
        
        var domainBinding = DomainBinding.Create(model.Domain, configuration);
        await domainRepository.AddAsync(domainBinding);
        
        Domains.Add(Map(domainBinding));
        EndAdd();
        StateHasChanged();
    }
}
