using DynDNS.Application.Components;
using DynDNS.Core;
using DynDNS.Core.Domains;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DynDNS.Application.Pages;

public sealed partial class Index(IDomainRepository domainRepository, IDialogService dialogService) : ComponentBase
{
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

    private Task OnChange(DomainBindingModel model)
    {
        throw new NotImplementedException();
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
        model.Subdomains.Remove(subdomain);
        
        StateHasChanged();
    }
}
