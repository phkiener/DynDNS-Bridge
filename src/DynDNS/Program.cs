using DynDNS;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureEndpoints();

app.Run();
