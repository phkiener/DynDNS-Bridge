using DynDNS.Zones;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DynDNS.WebInterface.Pages;

public sealed partial class Index(IZoneRepository zoneRepository, IDialogService dialogService) : ComponentBase
{
    private List<ZoneConfiguration> Zones { get; set; } = [];
    
    protected override void OnInitialized()
    {
        Zones = zoneRepository.GetZones().ToList();
    }

    private async Task DeleteZone(ZoneConfiguration zone)
    {
        var deletionConfirmed = await dialogService.ShowMessageBox(
            title: $"Deleting zone {zone.Zone}",
            message: "Are you sure you want to delete this zone?",
            yesText: "Yes",
            noText: "No");

        if (deletionConfirmed is true)
        {
            await zoneRepository.DeleteZoneAsync(zone.Zone, CancellationToken.None);
            Zones.Remove(zone);
        
            StateHasChanged();
        }
    }
}
