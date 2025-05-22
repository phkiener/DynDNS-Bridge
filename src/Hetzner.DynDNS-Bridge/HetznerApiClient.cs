using System.Text.Json.Serialization;

namespace Hetzner.DynDNSBridge;

/// <summary>
/// Full content of a DNS zone.
/// </summary>
/// <param name="Id">Id of the zone; used to add, update or remove records via <see cref="HetznerApiClient"/></param>
/// <param name="Entries">All records currently defined in this zone</param>
public sealed record DnsZone(string Id, DnsRecord[] Entries);

/// <summary>
/// An entry in a DNS zone.
/// </summary>
/// <param name="Id">Id of the entry; used to update or remove the record</param>
/// <param name="Type">Type of DNS record, e.g. <c>A</c> or <c>AAAA</c></param>
/// <param name="Name">Name of the record inside the zone</param>
/// <param name="Value">Value of the DNS record, e.g. the IPv4 address to point at for <c>A</c> records</param>
/// <remarks>
/// <paramref name="Name"/> is relative to the current zone. If a record named <c>example</c> is defined in zone <c>foobar.com</c>, it effectively
/// describes the domain <c>example.foobar.com</c>.
/// </remarks>
public sealed record DnsRecord(string Id, string Type, string Name, string Value);

/// <summary>
/// API client to interact with the Hetzner DNS Console.
/// </summary>
/// <param name="httpClient">The underlying <see cref="HttpClient"/> to use</param>
/// <remarks>
/// The <see cref="HttpClient"/> is expected to have the correct <see cref="HttpClient.BaseAddress"/> and authentication header set. No configuration
/// is done inside this class. Use <see cref="HetznerApiClient.ConfigureHttpClient"/> to get a fully configured client.
/// </remarks>
public sealed class HetznerApiClient(HttpClient httpClient)
{
    /// <summary>
    /// Load all information about a DNS zone.
    /// </summary>
    /// <param name="name">Name of the zone, e.g. <c>foobar.com</c></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    /// <returns>A <see cref="DnsZone"/> if a zone called <paramref name="name"/> exists or <c>null</c> otherwise</returns>
    public async Task<DnsZone?> GetZoneAsync(string name, CancellationToken cancellationToken)
    {
        var zoneResponse = await httpClient.GetFromJsonAsync<GetZonesResponse>($"zones?name={name}", cancellationToken: cancellationToken);
        var zoneId = zoneResponse?.Zones.SingleOrDefault()?.Id;
        if (zoneId is null)
        {
            return null;
        }
        
        var entryResponse = await httpClient.GetFromJsonAsync<GetRecordsResponse>($"records?zone_id={zoneId}", cancellationToken: cancellationToken);
        var entries = entryResponse?.Records.Select(static r => new DnsRecord(Id: r.Id, Type: r.Type, Name: r.Name, Value: r.Value)).ToArray()?? [];

        return new DnsZone(zoneId, entries);
    }

    /// <summary>
    /// Create a new DNS record in the given zone.
    /// </summary>
    /// <param name="zoneId">Id of the zone to create the record in</param>
    /// <param name="type">Type of record to create, e.g. <c>A</c> or <c>AAAA</c></param>
    /// <param name="name">Name of the record inside the zone</param>
    /// <param name="value">Value of the DNS record, e.g. the IPv4 address to point at for <c>A</c> records</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    /// <remarks>
    /// <paramref name="name"/> is relative to the current zone. If a record named <c>example</c> is defined in zone <c>foobar.com</c>, it effectively
    /// describes the domain <c>example.foobar.com</c>.
    /// </remarks>
    public async Task CreateRecordAsync(string zoneId, string type, string name, string value, CancellationToken cancellationToken)
    {
        var request = new CreateRecordRequest(zoneId, type, name, value);
        await httpClient.PostAsJsonAsync("records", request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Update an existing DNS record in the given zone.
    /// </summary>
    /// <param name="zoneId">Id of the zone to create the record in</param>
    /// <param name="recordId">Id of the record to update</param>
    /// <param name="type">Type of record, e.g. <c>A</c> or <c>AAAA</c></param>
    /// <param name="name">Name of the record inside the zone</param>
    /// <param name="value">Value of the DNS record, e.g. the IPv4 address to point at for <c>A</c> records</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    /// <remarks>
    /// <paramref name="name"/> is relative to the current zone. If a record named <c>example</c> is defined in zone <c>foobar.com</c>, it effectively
    /// describes the domain <c>example.foobar.com</c>.
    /// </remarks>
    public async Task UpdateRecordAsync(string zoneId, string? recordId, string type, string name, string value, CancellationToken cancellationToken)
    {
        var request = new CreateRecordRequest(zoneId, type, name, value);
        await httpClient.PutAsJsonAsync($"records/{recordId}", request, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Delete an existing DNS record.
    /// </summary>
    /// <param name="recordId">Id of the record to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    public async Task DeleteRecordAsync(string recordId, CancellationToken cancellationToken)
    {
        await httpClient.DeleteAsync($"records/{recordId}", cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// Fully configure the given <paramref name="httpClient"/> and construct a <see cref="HetznerApiClient"/> ready to be used.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> to configure</param>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> to fetch additional dependencies from</param>
    /// <returns>A fully constructed <see cref="HetznerApiClient"/></returns>
    public static HetznerApiClient ConfigureHttpClient(HttpClient httpClient, IServiceProvider serviceProvider)
    {
        httpClient.BaseAddress = new Uri("https://dns.hetzner.com/api/v1/");

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        httpClient.DefaultRequestHeaders.Add("Auth-API-Token", configuration.ApiKey());
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        return new HetznerApiClient(httpClient);
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

    private sealed class GetZonesResponse
    {
        [JsonRequired]
        [JsonPropertyName("zones")]
        public required Zone[] Zones { get; init; }
        
        public sealed class Zone
        {
            [JsonRequired]
            [JsonPropertyName("id")]
            public required string Id { get; init; }
        }
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
