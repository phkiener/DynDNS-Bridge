using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="plugins">The registered <see cref="IProviderPlugin"/>s.</param>
/// <param name="repository">The <see cref="IDomainBindingRepository"/> to use for persistence.</param>
public sealed class ProviderConfigurationsService(
    IEnumerable<IProviderPlugin> plugins,
    IDomainBindingRepository repository,
    ICurrentAddressProvider currentAddressProvider) : IProviderConfigurations
{
    private readonly Dictionary<string, IProviderPlugin> plugins = plugins.ToDictionary(static p => p.Name);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Providers => plugins.Keys;

    /// <inheritdoc />
    public IReadOnlyList<string> GetParameters(string provider)
    {
        return plugins.TryGetValue(provider, out var plugin)
            ? plugin.Parameters
            : throw new InvalidOperationException($"Unknown provider '{provider}'.");
    }

    /// <inheritdoc />
    public async Task ConfigureProviderAsync(DomainBindingId id, string provider, IReadOnlyDictionary<string, string> parameters)
    {
        var domainBinding = await repository.GetByIdAsync(id);
        if (domainBinding is null)
        {
            throw new InvalidOperationException($"DomainBinding {id} does not exist.");
        }

        var requiredParameters = plugins.TryGetValue(provider, out var plugin)
            ? plugin.Parameters
            : throw new InvalidOperationException($"Unknown provider '{provider}'.");

        if (requiredParameters.Count != parameters.Count
            || parameters.Keys.Intersect(requiredParameters).Count() != requiredParameters.Count)
        {
            throw new InvalidOperationException($"Invalid parameters for provider '{provider}'.");
        }

        domainBinding.ConfigureProvider(provider, parameters);
    }

    /// <inheritdoc />
    public async Task UpdateBindingsAsync(DomainBindingId id)
    {
        var domainBinding = await repository.GetByIdAsync(id);
        if (domainBinding is null)
        {
            throw new InvalidOperationException($"DomainBinding {id} does not exist.");
        }

        if (domainBinding.ProviderConfiguration is null)
        {
            throw new InvalidOperationException($"No provider is configured for {id}.");
        }

        if (!plugins.TryGetValue(domainBinding.ProviderConfiguration.Name, out var plugin))
        {
            throw new InvalidOperationException($"Unknown provider '{domainBinding.ProviderConfiguration.Name}'");
        }

        var client = plugin.GetClient(domainBinding.ProviderConfiguration.Parameters);
        var existingRecords = await client.GetRecordsAsync();

        var ipv4Address = await currentAddressProvider.GetIPv4AddressAsync();
        var ipv6Address = await currentAddressProvider.GetIPv6AddressAsync();

        foreach (var subdomain in domainBinding.Subdomains)
        {
            var ipv4 = existingRecords.SingleOrDefault(r => r.Hostname == domainBinding.Hostname && r.Subdomain == subdomain.Name && r.Type is DnsRecordType.A);
            if (subdomain.CreateIPv4Record && ipv4Address is not null)
            {
                await client.CreateRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.A, ipv4Address);
            }
            else if (ipv4 is not null && !subdomain.CreateIPv4Record)
            {
                await client.DeleteRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.A);
            }

            var ipv6 = existingRecords.SingleOrDefault(r => r.Hostname == domainBinding.Hostname && r.Subdomain == subdomain.Name && r.Type is DnsRecordType.AAAA);
            if (subdomain.CreateIPv6Record && ipv6Address is not null)
            {
                await client.CreateRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.AAAA, ipv6Address);
            }
            else if (ipv6 is not null && !subdomain.CreateIPv6Record)
            {
                await client.DeleteRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.AAAA);
            }
        }
    }
}
