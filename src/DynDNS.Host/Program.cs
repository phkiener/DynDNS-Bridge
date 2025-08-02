using DynDNS.Core;
using DynDNS.Host;
using DynDNS.Web;

var builder = WebApplication.CreateBuilder();
builder.Services.AddWebServices();
builder.Services.AddCoreServices();
builder.Configuration.When("DYNDNS_TRANSIENT", () => builder.Services.UseTransientCore(useScopes: false));

var app = builder.Build();
app.UseWebsite();
app.Run();
