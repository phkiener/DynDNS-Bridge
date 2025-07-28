using DynDNS.Core;
using Microsoft.JSInterop;

namespace DynDNS.Application.Components;

public sealed partial class CurrentAddressWidget(ICurrentAddressProvider currentAddress, IJSRuntime runtime) : IDisposable
{
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            currentAddress.AddressChanged += OnAddressChanged;
        }
    }

    void IDisposable.Dispose()
    {
        currentAddress.AddressChanged -= OnAddressChanged;
    }

    private void OnAddressChanged(object? sender, EventArgs eventArgs)
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task CopyToClipboard(string text)
    {
        await runtime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}
