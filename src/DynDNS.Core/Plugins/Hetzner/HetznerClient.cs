using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;
using Microsoft.Extensions.Logging;

namespace DynDNS.Core.Plugins.Hetzner;

internal sealed class HetznerClient(HttpClient client, string zoneId, string apiKey, ILogger logger) : IProviderClient
{
    public async Task CreateRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type, string ipAddress)
    {
        var request = new Models.CreateRecordRequest(zoneId, type.ToString(), subdomain, ipAddress);

        var currentRecord = await GetRecordIdAsync(subdomain, type);
        if (currentRecord is null)
        {
            logger.LogInformation("Creating {Type} record for {Subdomain}.{Hostname} as {Value}", type.ToString(), subdomain, hostname, ipAddress);
            await SendJsonAsync(HttpMethod.Post, "records", request);
        }
        else if (currentRecord.CurrentValue != ipAddress)
        {
            logger.LogInformation("Updating {Type} record for {Subdomain}.{Hostname} from {OldValue} to {Value}", type.ToString(), subdomain, hostname, currentRecord.CurrentValue, ipAddress);
            await SendJsonAsync(HttpMethod.Put, $"records/{currentRecord}", request);
        }
    }

    public async Task DeleteRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type)
    {
        var recordId = await GetRecordIdAsync(subdomain, type);
        if (recordId is not null)
        {
            logger.LogInformation("Deleting {Type} record for {Subdomain}.{Hostname}", type.ToString(), subdomain, hostname);
            await SendJsonAsync(HttpMethod.Delete, $"records/{recordId}");
        }
    }

    private async Task<FoundRecord?> GetRecordIdAsync(DomainFragment subdomain, DnsRecordType type)
    {
        var records = await SendJsonAsync<Models.GetRecordsResponse>(HttpMethod.Get, $"records?zone_id={zoneId}");
        foreach (var record in records?.Records ?? [])
        {
            if (record.Name == subdomain && record.Type == type.ToString())
            {
                return new FoundRecord(record.Id, record.Value);
            }
        }

        return null;
    }

    private async Task SendJsonAsync(HttpMethod method, string route, object? body = null)
    {
        var request = new HttpRequestMessage(method, route);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
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
        }

        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Auth-API-Token", apiKey);

        var response = await client.SendAsync(request);
        return await response.Content.ReadFromJsonAsync<TOut>();
    }

    private sealed record FoundRecord(string Id, string CurrentValue);

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
