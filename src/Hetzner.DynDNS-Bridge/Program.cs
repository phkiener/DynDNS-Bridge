using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

const string HttpClientName = "DNS Console";
const string EnvPrefix = "DDNSBRIDGE_";
const string ApiKeyVariable = $"{EnvPrefix}API_KEY";

var builder = WebApplication.CreateSlimBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: EnvPrefix);
builder.Services.AddHttpClient(HttpClientName, ConfigureHttpClient);

var host = builder.Build();
host.MapGet("update/{zone}/{name}", Invoke);
host.Run();

return;

static void ConfigureHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
{
    httpClient.BaseAddress = new Uri("https://dns.hetzner.com/api/v1/");
    
    var apiKey = serviceProvider.GetRequiredService<IConfiguration>()[ApiKeyVariable] ?? throw new InvalidOperationException("Missing API Key");
    httpClient.DefaultRequestHeaders.Add("Auth-API-Token", apiKey);
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
}

static async Task Invoke(
    HttpContext httpContext,
    [FromServices] IHttpClientFactory httpClientFactory,
    [FromRoute(Name = "zone")] string zone,
    [FromRoute(Name = "name")] string name,
    [FromQuery(Name = "v4")] string? ipv4 = null,
    [FromQuery(Name = "v6")] string? ipv6 = null)
{
    var client = httpClientFactory.CreateClient(HttpClientName);
    var existingRecords = await client.GetFromJsonAsync<AllDnsRecords>($"records?zone_id={zone}", httpContext.RequestAborted);
    if (existingRecords is null)
    {
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.CompleteAsync();
        return;
    }

    var existingIpv4Record = existingRecords.Records.SingleOrDefault(r => r.Name == name && r.Type is "A");
    await UpdateRecord(zone, name, "A", existingIpv4Record, ipv4, client, httpContext.RequestAborted);

    var existingIpv6Record = existingRecords.Records.SingleOrDefault(r => r.Name == name && r.Type is "AAAA");
    await UpdateRecord(zone, name, "AAAA", existingIpv6Record, ipv6, client, httpContext.RequestAborted);

    httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
    await httpContext.Response.CompleteAsync();
}

static async Task UpdateRecord(string zone, string name, string type, DnsRecord? record, string? value, HttpClient client, CancellationToken cancellationToken)
{
    if (value is null && record is not null)
    {
        await client.DeleteAsync($"$records/{record.Id}",cancellationToken);
    }
    else if (value is not null)
    {
        var request = new CreateDnsRecord(zone, type, name, value);
        if (record is null)
        {
            await client.PostAsJsonAsync("records", request, cancellationToken);
        }
        else if (record.Value != value)
        {
            await client.PutAsJsonAsync($"records/{record.Id}", request, cancellationToken);
        }
    }
}

sealed record AllDnsRecords(DnsRecord[] Records);
sealed record DnsRecord(string Type, string Id, string Name, string Value);

sealed class CreateDnsRecord(string zone, string type, string name, string value)
{
    [JsonPropertyName("zone_id")]
    public string Zone { get; } = zone;
    
    [JsonPropertyName("type")]
    public string Type { get; } = type;
    
    [JsonPropertyName("name")]
    public string Name { get; } = name;
    
    [JsonPropertyName("value")]
    public string Value { get; } = value;
}
