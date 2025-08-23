using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;
using DomainBinding = DynDNS.Core.Model.DomainBinding;

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
        await repository.UpdateAsync(domainBinding);
    }

    /// <inheritdoc />
    public async Task UpdateBindingsAsync(DomainBindingId id)
    {
        var domainBinding = await repository.GetByIdAsync(id);
        if (domainBinding is null)
        {
            throw new InvalidOperationException($"DomainBinding {id} does not exist.");
        }

        var ipv4Address = await currentAddressProvider.GetIPv4AddressAsync();
        var ipv6Address = await currentAddressProvider.GetIPv6AddressAsync();

        await UpdateBindingAsync(domainBinding, ipv4Address, ipv6Address);
    }

    public async Task UpdateAllBindingsAsync(CancellationToken cancellationToken)
    {
        var ipv4Address = await currentAddressProvider.GetIPv4AddressAsync();
        var ipv6Address = await currentAddressProvider.GetIPv6AddressAsync();

        var domainBindings = await repository.GetAllAsync();
        foreach (var binding in domainBindings)
        {
            if (binding.ProviderConfiguration is null)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            await UpdateBindingAsync(binding, ipv4Address, ipv6Address);
        }
    }

    private async Task UpdateBindingAsync(DomainBinding domainBinding, string? ipv4Address, string? ipv6Address)
    {
        if (domainBinding.ProviderConfiguration is null)
        {
            throw new InvalidOperationException($"No provider is configured for {domainBinding.Id}.");
        }

        if (!plugins.TryGetValue(domainBinding.ProviderConfiguration.Name, out var plugin))
        {
            throw new InvalidOperationException($"Unknown provider '{domainBinding.ProviderConfiguration.Name}'");
        }

        var client = plugin.GetClient(domainBinding.ProviderConfiguration.Parameters);
        foreach (var subdomain in domainBinding.Subdomains)
        {
            if (subdomain.CreateIPv4Record && ipv4Address is not null)
            {
                await client.CreateRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.A, ipv4Address);
            }
            else if (!subdomain.CreateIPv4Record)
            {
                await client.DeleteRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.A);
            }

            if (subdomain.CreateIPv6Record && ipv6Address is not null)
            {
                await client.CreateRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.AAAA, ipv6Address);
            }
            else if (!subdomain.CreateIPv6Record)
            {
                await client.DeleteRecordAsync(domainBinding.Hostname, subdomain.Name, DnsRecordType.AAAA);
            }
        }
    }
}
