using Microsoft.AspNetCore.Components;

namespace DynDNS.WebInterface.Primitives;

public sealed partial class DynButton : ComponentBase
{
    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }
    
    [Parameter]
    public bool Disabled { get; set; }
    
    [Parameter]
    public bool Submit { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
