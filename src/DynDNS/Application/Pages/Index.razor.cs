using DynDNS.Core;
using DynDNS.Core.Domains;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DynDNS.Application.Pages;

public sealed partial class Index(IDomainRepository domainRepository, IDialogService dialogService) : ComponentBase
{
    private List<DomainBinding> Domains { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Domains = (await domainRepository.GetAllAsync()).ToList();
    }

    private async Task DeleteDomain(DomainBinding domain)
    {
        var deletionConfirmed = await dialogService.ShowMessageBox(
            title: $"Deleting zone {domain.Id}",
            message: "Are you sure you want to delete this domain?",
            yesText: "Yes",
            noText: "No");

        if (deletionConfirmed is true)
        {
            await domainRepository.DeleteAsync(domain);
            Domains.Remove(domain);
        
            StateHasChanged();
        }
    }
}
