using Microsoft.AspNetCore.Components;

namespace DynDNS.WebInterface;

public sealed partial class Zone : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Name { get; set; }
    
    [Parameter, EditorRequired]
    public required string Provider { get; set; }

    [Parameter, EditorRequired]
    public required Dictionary<string, string> Parameters { get; set; }

    [Parameter, EditorRequired]
    public required List<string> Subdomains { get; set; }
}
