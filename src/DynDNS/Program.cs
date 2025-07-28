using DynDNS.Framework;
using DynDNS.Framework.Network;
using DynDNS.WebInterface;
using DynDNS.Zones;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true);

builder.Services.AddHttpClient();
builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IZoneRepository, ZoneRepository>();
builder.Services.AddSingleton<CurrentAddressProvider>();
builder.Services.AddSingleton<ICurrentAddressProvider>(static sp => sp.GetRequiredService<CurrentAddressProvider>());
builder.Services.AddHostedService(static sp => sp.GetRequiredService<CurrentAddressProvider>());

var app = builder.Build();
app.MapStaticAssets();
app.MapRazorComponents<_Root>()
    .AddInteractiveServerRenderMode()
    .DisableAntiforgery();

app.Run();
