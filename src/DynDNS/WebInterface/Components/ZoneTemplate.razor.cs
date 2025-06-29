using DynDNS.Zones;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DynDNS.WebInterface.Components;

public sealed partial class ZoneTemplate(IZoneRepository repository) : Zone, IDisposable
{
    private EditContext editContext = null!;
    private ValidationMessageStore validationMessages = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        editContext = new EditContext(this);
        editContext.OnValidationRequested += OnValidate;
        validationMessages = new ValidationMessageStore(editContext);
    }

    private void OnValidate(object? sender, ValidationRequestedEventArgs e)
    {
        validationMessages.Clear();
        if (!Validate)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            validationMessages.Add(() => Name, "Name is required.");
        }
        else if (repository.GetZones().Any(z => z.Zone == Name))
        {
            validationMessages.Add(() => Name, $"Zone '{Name}' already exists.");
        }
    }

    [SupplyParameterFromQuery]
    public bool Validate { get; set; }

    public void Dispose()
    {
        editContext.OnValidationRequested -= OnValidate;
    }
}
