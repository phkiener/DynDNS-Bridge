using Microsoft.AspNetCore.Components;
using ConfigurationProvider = DynDNS.Providers.ConfigurationProvider;

namespace DynDNS.Application.Components;

public sealed record CreateDomainBindingModel(
    string Provider,
    string Domain,
    IReadOnlyDictionary<string, string> Parameters);

public sealed partial class CreateDomainBindingCard(ConfigurationProvider configurationProvider) : ComponentBase
{
    private string? provider = null;
    private string domain = "";
    private Dictionary<string, string>? parameters = null;
    private bool canSubmit = false;
    
    [Parameter]
    public EventCallback<CreateDomainBindingModel> OnSubmit { get; set; }
    
    [Parameter]
    public EventCallback OnCancel { get; set; }

    private void LoadConfiguration()
    {
        if (provider is not null)
        {
            parameters = configurationProvider.Default(provider).Snapshot().ToDictionary();
        }
    }

    private void ValidateSubmit()
    {
        canSubmit = provider is not null
                    && !string.IsNullOrWhiteSpace(domain)
                    && parameters is not null
                    && parameters.Any(static kvp => !string.IsNullOrWhiteSpace(kvp.Value));
    }

    private Task SubmitBinding()
    {
        if (provider is not null && parameters is not null)
        {
            var model = new CreateDomainBindingModel(provider, domain, parameters);
            return OnSubmit.InvokeAsync(model);
        }
        
        return Task.CompletedTask;
    }

    private Task CancelBinding()
    {
        return OnCancel.InvokeAsync();
    }
}
