namespace DynDNS;

/// <summary>
/// An URL to configure for DynDNS.
/// </summary>
/// <param name="Zone">Zone in which to create a DNS record, e.g. <c>foobar.com</c></param>
/// <param name="Name">Name of the DNS record, e.g. <c>blog</c></param>
/// <remarks>
/// The resulting domain will be of the form <c>{Name}.{Zone}</c>, e.g. <c>blog.foobar.com</c>.
/// </remarks>
public readonly record struct ConfiguredUrl(string Zone, string Name)
{
    public override string ToString() => $"{Name}.{Zone}";
}

/// <summary>
/// Helpers to extract required information from an <see cref="IConfiguration"/>.
/// </summary>
public static class Configuration
{
    /// <summary>
    /// Load the API key for the Hetzner DNS console from the given configuration.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> from which to load the API key</param>
    /// <returns>The found API key</returns>
    /// <exception cref="InvalidOperationException">No API key was found in the configuration</exception>
    public static string ApiKey(this IConfiguration configuration)
    {
        const string key = "DDNSBRIDGE_APIKEY";
        return configuration[key]?? throw new InvalidOperationException($"API key ({key}) is missing from configuration.");
    }

    /// <summary>
    /// Load all <see cref="ConfiguredUrl"/>s that are configured.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> from which to load the URLs</param>
    /// <returns>All found URLs</returns>
    /// <exception cref="InvalidOperationException">No URLs have been configured or at least one URL is in an invalid format</exception>
    public static IEnumerable<ConfiguredUrl> Urls(this IConfiguration configuration)
    {
        const string key = "DDNSBRIDGE_DOMAINS";
        return configuration[key]?.Split(';').Select(ParseUrl) ?? throw new InvalidOperationException($"URLs ({key}) are missing from configuration.");
    }

    private static ConfiguredUrl ParseUrl(string rawUrl)
    {
        if (!Uri.TryCreate($"https://{rawUrl}", UriKind.Absolute, out _))
        {
            throw new InvalidOperationException($"URL ({rawUrl}) is not a valid URL.");
        }
        
        var parts = rawUrl.Split('.');
        var zoneName = string.Join('.', parts[^2..]);
        var domainName = string.Join('.', parts[..^2]);

        return new ConfiguredUrl(Zone: zoneName, Name: domainName);
    }
}
