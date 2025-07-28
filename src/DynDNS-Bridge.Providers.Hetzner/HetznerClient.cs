using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DynDNS.Providers.Hetzner;

public sealed class HetznerClient(IHttpClientFactory httpClientFactory) : IProviderClient
{
    public string Name => "Hetzner";
    
    public async Task ApplyAsync(DomainBindingConfiguration configuration, IReadOnlyCollection<string> subdomains, string? targetAddressIPv4, string? targetAddressIPv6)
    {
        if (configuration is not HetznerBindingConfiguration hetznerBindingConfiguration)
        {
            throw new ArgumentException($"Configuration must be of type {nameof(HetznerBindingConfiguration)}", nameof(configuration));
        }
        
        var client = httpClientFactory.CreateClient(nameof(HetznerClient));
        var currentState = await GetRecordsAsync(client, hetznerBindingConfiguration.ZoneId, hetznerBindingConfiguration.ApiKey);

        foreach (var subdomain in subdomains)
        {
            var ipv4Entry = currentState.Records.SingleOrDefault(r => r.Name == subdomain && r.Type == "A");
            var ipv6Entry = currentState.Records.SingleOrDefault(r => r.Name == subdomain && r.Type == "AAAA");
            
            await UpdateRecordAsync(client, hetznerBindingConfiguration.ZoneId, hetznerBindingConfiguration.ApiKey, subdomain, "A", ipv4Entry, targetAddressIPv4);
            await UpdateRecordAsync(client, hetznerBindingConfiguration.ZoneId, hetznerBindingConfiguration.ApiKey, subdomain, "AAAA", ipv6Entry, targetAddressIPv6);
        }
    }

    private static async Task<GetRecordsResponse> GetRecordsAsync(HttpClient httpClient, string zoneId, string apiKey)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"records?zone_id={zoneId}");
        request.Headers.Add("Auth-API-Token", apiKey);

        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadFromJsonAsync<GetRecordsResponse>()
               ?? throw new InvalidOperationException($"Failed to fetch existing entries for zone {zoneId}");
    }

    private static async Task UpdateRecordAsync(
        HttpClient httpClient,
        string zoneId,
        string apiKey,
        string name,
        string type,
        GetRecordsResponse.Record? existingRecord,
        string? targetAddress)
    {
        if (existingRecord is null && targetAddress is null)
        {
            return;
        }

        if (existingRecord is null && targetAddress is not null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "records");
            request.Headers.Add("Auth-API-Token", apiKey);
            request.Content = JsonContent.Create(new CreateRecordRequest(zoneId, type, name, targetAddress));
            
            await httpClient.SendAsync(request);
        }

        if (existingRecord is not null && targetAddress is null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"records/{existingRecord.Id}");
            await httpClient.SendAsync(request);
        }

        if (existingRecord is not null && targetAddress is not null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, $"records/{existingRecord.Id}");
            request.Headers.Add("Auth-API-Token", apiKey);
            request.Content = JsonContent.Create(new CreateRecordRequest(zoneId, type, name, targetAddress));
            
            await httpClient.SendAsync(request);
        }
    }

    private sealed class CreateRecordRequest(string zoneId, string type, string name, string value)
    {
        [JsonPropertyName("zone_id")]
        public string Zone { get; } = zoneId;
    
        [JsonPropertyName("type")]
        public string Type { get; } = type;
    
        [JsonPropertyName("name")]
        public string Name { get; } = name;
    
        [JsonPropertyName("value")]
        public string Value { get; } = value;
    }

    private sealed class GetRecordsResponse
    {
        [JsonRequired]
        [JsonPropertyName("records")]
        public required Record[] Records { get; init; }
        
        public sealed class Record
        {
            [JsonRequired]
            [JsonPropertyName("id")]
            public required string Id { get; init; }
            
            [JsonRequired]
            [JsonPropertyName("type")]
            public required string Type { get; init; }
            
            [JsonRequired]
            [JsonPropertyName("name")]
            public required string Name { get; init; }
            
            [JsonRequired]
            [JsonPropertyName("value")]
            public required string Value { get; init; }
        }
    }
}
