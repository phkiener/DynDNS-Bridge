using Microsoft.AspNetCore.Components;

namespace DynDNS.WebInterface.Components;

public sealed partial class Zone : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Name { get; set; }
    
    [Parameter, EditorRequired]
    public required string Provider { get; set; }

    [Parameter, EditorRequired]
    public required IReadOnlyDictionary<string, string> Parameters { get; set; }

    [Parameter, EditorRequired]
    public required IReadOnlyList<string> Subdomains { get; set; }
}
