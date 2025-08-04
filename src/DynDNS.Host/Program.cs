using DynDNS.Core;
using DynDNS.Core.Plugins.Hetzner;
using DynDNS.Host;
using DynDNS.Web;

var builder = WebApplication.CreateBuilder();
builder.Services.AddWebServices();
builder.Services.AddCoreServices();
builder.Services.AddHetznerPlugin();
builder.Configuration.When("DYNDNS_TRANSIENT", () => builder.Services.UseTransientCore(useScopes: false));
builder.Services.AddHostedService<CurrentAddressHostedService>();

var app = builder.Build();
app.UseWebsite();
app.Run();
