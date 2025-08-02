namespace DynDNS.Host;

public static class ConfigurationExtensions
{
    public static void When(this IConfiguration configuration, string key, Action action)
    {
        if (configuration[key] is not null)
        {
            action.Invoke();
        }
    }
}
