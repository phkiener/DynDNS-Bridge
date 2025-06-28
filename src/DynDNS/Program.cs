using DynDNS.Network;
using DynDNS.WebInterface;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.user.json",  optional: true, reloadOnChange: true);
builder.Services.AddRazorComponents();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<CurrentAddressProvider>();
builder.Services.AddSingleton<ICurrentAddressProvider>(static sp => sp.GetRequiredService<CurrentAddressProvider>());
builder.Services.AddHostedService(static sp => sp.GetRequiredService<CurrentAddressProvider>());

var app = builder.Build();
app.MapStaticAssets();
app.UseAntiforgery();
app.MapRazorComponents<Root>();
app.Run();
