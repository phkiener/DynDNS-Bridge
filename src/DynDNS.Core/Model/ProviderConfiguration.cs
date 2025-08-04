using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.Model;

/// <summary>
/// Configuration for a provider to create, update or delete DNS records for a <see cref="DomainBinding"/>.
/// </summary>
/// <param name="Name">Name of the provider; must be a valid <see cref="IProviderPlugin"/>.</param>
/// <param name="Parameters">All parameters to pass to the provider client.</param>
public sealed record ProviderConfiguration(string Name, IReadOnlyDictionary<string, string> Parameters);
