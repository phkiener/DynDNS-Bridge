using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.UseCases;

/// <inheritdoc />
/// <param name="plugins">The registered <see cref="IProviderPlugin"/>s.</param>
/// <param name="repository">The <see cref="IDomainBindingRepository"/> to use for persistence.</param>
public sealed class ProviderConfigurationsService(IEnumerable<IProviderPlugin> plugins, IDomainBindingRepository repository) : IProviderConfigurations
{
    private readonly Dictionary<string, IProviderPlugin> plugins = plugins.ToDictionary(static p => p.Name);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Providers => plugins.Keys;

    /// <inheritdoc />
    public IReadOnlyList<string> GetParameters(string provider)
    {
        return plugins.TryGetValue(provider, out var plugin)
            ? plugin.Parameters
            : throw new InvalidOperationException($"Unknown provider '{provider}'");
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
            : throw new InvalidOperationException($"Unknown provider '{provider}'");

        if (requiredParameters.Count != parameters.Count
            || parameters.Keys.Intersect(requiredParameters).Count() != requiredParameters.Count)
        {
            throw new InvalidOperationException($"Invalid parameters for provider '{provider}'");
        }

        domainBinding.ConfigureProvider(provider, parameters);
    }
}
