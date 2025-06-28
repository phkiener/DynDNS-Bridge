namespace DynDNS;

/// <summary>
/// An updater for DNS records to make sure they are in the required state.
/// </summary>
/// <param name="apiClient">The <see cref="HetznerApiClient"/> to use for updating the DNS records</param>
/// <param name="logger">The <see cref="ILogger"/> to use for logging</param>
public sealed class DnsEntryUpdater(HetznerApiClient apiClient, ILogger<DnsEntryUpdater> logger)
{
    private readonly DnsZoneCache zoneCache = new(apiClient);

    /// <summary>
    /// Update the DNS record defined by <paramref name="url"/> for type <paramref name="type"/> to point at <paramref name="ip"/> - or delete it if
    /// <see cref="ip"/> is <c>null</c>.
    /// </summary>
    /// <param name="type">Type of DNS record to update or delete</param>
    /// <param name="ip">The IP address to point at; pass <c>null</c> to delete the DNS record</param>
    /// <param name="url">The <see cref="ConfiguredUrl"/> for which to update or delete the record</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the operation</param>
    /// <exception cref="InvalidOperationException">No zone was found for the given <paramref name="url"/></exception>
    public async Task UpdateEntryAsync(string type, string? ip, ConfiguredUrl url, CancellationToken cancellationToken)
    {
        var zone = await zoneCache.GetZoneAsync(url.Zone, cancellationToken);
        if (zone is null)
        {
            throw new InvalidOperationException($"Zone not found: {url.Zone}.");
        }

        var existingEntry = zone.Entries.SingleOrDefault(r => r.Type == type && r.Name == url.Name);
        if (ip is null && existingEntry is not null)
        {
            logger.LogInformation("Deleting entry for {Url} because no IP for type {Type} has been passed", url, type);
            await apiClient.DeleteRecordAsync(existingEntry.Id, cancellationToken);
        }

        if (ip is not null && existingEntry is null)
        {
            logger.LogInformation("Creating entry for {Url} because none exists for type {Type}", url, type);
            await apiClient.CreateRecordAsync(
                zoneId: zone.Id,
                type: type,
                name: url.Name,
                value: ip,
                cancellationToken: cancellationToken);
        }

        if (ip is not null && existingEntry is not null && existingEntry.Value != ip)
        {
            logger.LogInformation("Updating entry for {Url} because IP of type {Type} changed", url, type);
            await apiClient.UpdateRecordAsync(
                zoneId: zone.Id,
                recordId: existingEntry.Id,
                type: type,
                name: url.Name,
                value: ip,
                cancellationToken: cancellationToken);
        }
    }
}
