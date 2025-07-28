using Microsoft.AspNetCore.Components;

namespace DynDNS.Application.Components;

public sealed class DomainBindingModel
{
    public required string Provider { get; init; }
    public required string Domain { get; init; }
    
    public bool BindIPv4 { get; set; }
    public bool BindIPv6 { get; set; }

    public List<string> Subdomains { get; set; } = [];
    public Dictionary<string, string> Parameters { get; set; } = [];
}

public sealed partial class DomainBindingCard : ComponentBase
{
    private bool isAdding = false;
    private string addedSubdomain = "";

    private string? editingKey = null;
    private string editedValue = "";
    
    [Parameter]
    [EditorRequired]
    public required DomainBindingModel Model { get; set; }
    
    [Parameter]
    public EventCallback OnApply { get; set; }
    
    [Parameter]
    public EventCallback OnDelete { get; set; }
    
    [Parameter]
    public EventCallback<(string Key, string Value)> OnConfigEdited { get; set; }
    
    [Parameter]
    public EventCallback<string> OnSubdomainAdded { get; set; }
    
    [Parameter]
    public EventCallback<string> OnSubdomainRemoved { get; set; }
    
    private Task DeleteDomain()
    {
        return OnDelete.InvokeAsync();
    }
    
    private async Task AddSubdomain(string subdomain)
    {
        await OnSubdomainAdded.InvokeAsync(subdomain);
        EndAddSubdomain();
    }
    
    private Task RemoveSubdomain(string subdomain)
    {
        return OnSubdomainRemoved.InvokeAsync(subdomain);
    }

    private void BeginAddSubdomain()
    {
        addedSubdomain = "";
        isAdding = true;
    }

    private void EndAddSubdomain()
    {
        addedSubdomain = "";
        isAdding = false;
    }
    
    private async Task EditConfig(string key)
    {
        await OnConfigEdited.InvokeAsync((key, editedValue));
        EndEditConfig();
    }

    private void BeginEditConfig(string key)
    {
        editingKey = key;
        editedValue = Model.Parameters[key];
    }

    private void EndEditConfig()
    {
        editingKey = null;
        editedValue = "";
    }

    private Task AppyBinding()
    {
        return OnApply.InvokeAsync();
    }
}
