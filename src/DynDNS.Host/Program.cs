using DynDNS.Core;
using DynDNS.Host;
using DynDNS.Web;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder();

builder.Services.AddMudServices();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddCoreServices();
if (builder.Configuration.UseTransient())
{
    builder.Services.UseTransientCore(useScopes: false);
}

var app = builder.Build();
app.MapStaticAssets();
app.UseStatusCodePagesWithReExecute("/status/{0}");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .DisableAntiforgery();

app.Run();
