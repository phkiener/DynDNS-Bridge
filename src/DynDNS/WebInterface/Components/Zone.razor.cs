using Microsoft.AspNetCore.Components;

namespace DynDNS.WebInterface.Components;

public partial class Zone : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Name { get; set; }
    
    [Parameter, EditorRequired]
    public required string Provider { get; set; }
    
    [Parameter, EditorRequired]
    public required bool BindIPv4 { get; set; }
    
    [Parameter, EditorRequired]
    public required bool BindIPv6 { get; set; }
    
    [Parameter, EditorRequired]
    public required string ApiKey { get; set; }

    [Parameter, EditorRequired]
    public required IReadOnlyList<string> Subdomains { get; set; }
}
