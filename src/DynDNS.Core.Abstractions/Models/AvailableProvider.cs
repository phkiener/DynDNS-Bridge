namespace DynDNS.Core.Abstractions.Models;

/// <summary>
/// A provider that is configured to handle creation and deletion of DNS records.
/// </summary>
/// <param name="Name">Name of the provider.</param>
/// <param name="ConfigurationParameters">The parameters available when configuring this provider.</param>
public sealed record AvailableProvider(string Name, IReadOnlyCollection<ProviderConfigurationParameter> ConfigurationParameters);

/// <summary>
/// A parameter that is part of the configuration for a provider.
/// </summary>
/// <param name="Name">Name of the parameter.</param>
/// <param name="Required">Whether it is a required parameter.</param>
/// <param name="Type">The <see cref="ParameterType"/> of the parameter.</param>
public sealed record ProviderConfigurationParameter(string Name, bool Required, ParameterType Type);

/// <summary>
/// The type of configuration parameter.
/// </summary>
public enum ParameterType
{
    /// <summary>
    /// A textual parameter, i.e. a string.
    /// </summary>
    Text
}
