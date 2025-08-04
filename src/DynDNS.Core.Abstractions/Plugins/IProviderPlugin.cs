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

    /// <summary>
    /// Return a <see cref="IProviderClient"/> to use based on the given <see cref="parameters"/>.
    /// </summary>
    /// <param name="parameters">The parameters with which to configure the client.</param>
    /// <returns>The fully built client.</returns>
    IProviderClient GetClient(IReadOnlyDictionary<string, string> parameters);
}
