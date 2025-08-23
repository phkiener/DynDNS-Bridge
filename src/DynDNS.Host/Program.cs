using DynDNS.Core;
using DynDNS.Core.Plugins.Hetzner;
using DynDNS.Host;
using DynDNS.Host.Jobs;
using DynDNS.Web;

var builder = WebApplication.CreateBuilder();
builder.Services.AddWebServices();
builder.Services.AddCoreServices();
builder.Services.AddHetznerPlugin();
builder.Services.AddHostedService<CurrentAddressHostedService>();
builder.Configuration.When("DYNDNS_TRANSIENT", () => builder.Services.UseTransientCore(useScopes: false));
builder.Configuration.When("DYNDNS_SCHEDULE", () => builder.Services.AddHostedService<ScheduledBindingUpdater>());

var app = builder.Build();
app.UseWebsite();
app.Run();
