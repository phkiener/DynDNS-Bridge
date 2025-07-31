namespace DynDNS.Host;

internal static class ConfigurationExtensions
{
    public static bool UseTransient(this IConfiguration configuration)
    {
        return configuration["DYNDNS_TRANSIENT"] is not null;
    }
}
