namespace DynDNS.Core.Abstractions.Plugins;

/// <summary>
/// The plugin for a domain provider.
/// </summary>
public interface IProviderPlugin
{
    /// <summary>
    /// Name of the provider.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A list of parameters to be configured.
    /// </summary>
    IReadOnlyList<string> Parameters { get; }
}
