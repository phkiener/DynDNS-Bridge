using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.Plugins.Hetzner;

internal sealed class HetznerClient(HttpClient client, string zoneId, string apiKey) : IProviderClient
{
    private sealed record DnsRecordKey(string ZoneId, string Name, string Type);

    private static readonly ConcurrentDictionary<DnsRecordKey, string> recordIdByKey = new();

    public async Task CreateRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type, string ipAddress)
    {
        var request = new Models.CreateRecordRequest(zoneId, type.ToString(), subdomain, ipAddress);

        var recordId = await GetRecordIdAsync(subdomain, type);
        if (recordId is null)
        {
            await SendJsonAsync(HttpMethod.Post, "records", request);
        }
        else
        {
            await SendJsonAsync(HttpMethod.Put, $"records/{recordId}", request);
        }
    }

    public async Task DeleteRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type)
    {
        var recordId = await GetRecordIdAsync(subdomain, type);
        if (recordId is not null)
        {
            await SendJsonAsync(HttpMethod.Delete, $"records/{recordId}");
        }
    }

    private async Task<string?> GetRecordIdAsync(DomainFragment subdomain, DnsRecordType type)
    {
        var recordKey = new DnsRecordKey(zoneId, subdomain, type.ToString());
        if (recordIdByKey.TryGetValue(recordKey, out var value))
        {
            return value;
        }

        var records = await SendJsonAsync<Models.GetRecordsResponse>(HttpMethod.Get, $"records?zone_id={zoneId}");
        foreach (var record in records?.Records ?? [])
        {
            var key = new DnsRecordKey(zoneId, record.Name, record.Type);
            recordIdByKey[key] = record.Id;
        }

        return recordIdByKey.GetValueOrDefault(recordKey);
    }

    private async Task SendJsonAsync(HttpMethod method, string route, object? body = null)
    {
        var request = new HttpRequestMessage(method, route);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
            request.Headers.Add("Content-Type", "application/json");
        }

        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Auth-API-Token", apiKey);

        await client.SendAsync(request);
    }

    private async Task<TOut?> SendJsonAsync<TOut>(HttpMethod method, string route, object? body = null)
    {
        var request = new HttpRequestMessage(method, route);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
            request.Headers.Add("Content-Type", "application/json");
        }

        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Auth-API-Token", apiKey);

        var response = await client.SendAsync(request);
        return await response.Content.ReadFromJsonAsync<TOut>();
    }

    private static class Models
    {
        public sealed class CreateRecordRequest(string zoneId, string type, string name, string value)
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

        public sealed class GetRecordsResponse
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
}
