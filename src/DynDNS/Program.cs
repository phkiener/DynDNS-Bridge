using DynDNS.Application;
using DynDNS.Application.Endpoints;
using DynDNS.Core;
using DynDNS.Providers;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true);

builder.Services.AddCoreServices();
builder.Services.AddDomainProviders().AddHetzner();
builder.Services.AddHttpClient();
builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
app.MapStaticAssets();

app.Map("api/v1/refresh", ApplyBindingsEndpoint.Handle);
app.MapRazorComponents<_Root>()
    .AddInteractiveServerRenderMode()
    .DisableAntiforgery();

app.Run();
