using DynDNS.WebInterface;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.user.json",  optional: true, reloadOnChange: true);
builder.Services.AddRazorComponents();

var app = builder.Build();
app.MapStaticAssets();
app.UseAntiforgery();
app.MapRazorComponents<Root>();
app.Run();
