using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;
using Microsoft.Extensions.Logging;

namespace DynDNS.Core.Plugins.HetznerCloud;

internal sealed class HetznerCloudClient(HttpClient client, ILogger logger) : IProviderClient
{
    public async Task CreateRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type, string ipAddress)
    {
        var targetRecord = new Models.Record { Value = ipAddress, Comment = "Managed by dyndns-bridge" };

        var recordSet = await SendJsonAsync<Models.GetResourceRecordSetResponse>(HttpMethod.Get, $"zones/{hostname}/rrsets/{subdomain}/{type}");
        if (recordSet is null)
        {
            logger.LogInformation("Creating {Type} record for {Subdomain}.{Hostname} as {Value}", type.ToString(), subdomain, hostname, ipAddress);

            var request = new Models.AddRecordsRequest([targetRecord]);
            await SendJsonAsync(HttpMethod.Post, $"zones/{hostname}/rrsets/{subdomain}/{type}/actions/add_records", request);

        }
        else if (recordSet.ResourceRecordSet.Records is not [var singleRecord] || singleRecord != targetRecord)
        {
            logger.LogInformation("Updating {Type} record for {Subdomain}.{Hostname} to {Value}", type.ToString(), subdomain, hostname, ipAddress);

            var request = new Models.SetRecordsRequest([targetRecord]);
            await SendJsonAsync(HttpMethod.Post, $"zones/{hostname}/rrsets/{subdomain}/{type}/actions/set_records", request);
        }
    }

    public async Task DeleteRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type)
    {
        await SendJsonAsync(HttpMethod.Delete, $"zones/{hostname}/rrsets/{subdomain}/{type}");
    }

    private async Task SendJsonAsync(HttpMethod method, string route, object? body = null)
    {
        var request = new HttpRequestMessage(method, route);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        await client.SendAsync(request);
    }

    private async Task<TOut?> SendJsonAsync<TOut>(HttpMethod method, string route, object? body = null)
    {
        var request = new HttpRequestMessage(method, route);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        var response = await client.SendAsync(request);
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return default;
        }

        var returnedBody = await response.Content.ReadAsStringAsync();

        return await response.Content.ReadFromJsonAsync<TOut>();
    }

    private static class Models
    {
        public sealed class AddRecordsRequest(IEnumerable<Record> records)
        {
            [JsonPropertyName("records")]
            public Record[] Records { get; } = records.ToArray();
        }

        public sealed class SetRecordsRequest(IEnumerable<Record> records)
        {
            [JsonPropertyName("records")]
            public Record[] Records { get; } = records.ToArray();
        }

        public sealed class GetResourceRecordSetResponse
        {
            [JsonRequired]
            [JsonPropertyName("rrset")]
            public required ResourceRecordSet ResourceRecordSet { get; init; }
        }

        public sealed class ResourceRecordSet
        {
            [JsonRequired]
            [JsonPropertyName("records")]
            public required Record[] Records { get; init; }
        }

        public sealed record Record
        {
            [JsonRequired]
            [JsonPropertyName("value")]
            public required string Value { get; set; }

            [JsonPropertyName("comment")]
            public string? Comment { get; init; }
        }
    }
}
