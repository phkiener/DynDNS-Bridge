namespace DynDNS.Providers;

public sealed class ConfigurationProvider(IEnumerable<IProvider> providers)
{
    public IEnumerable<string> AvailableProviders()
    {
        return providers.Select(static p => p.Name);
    }

    public DomainBindingConfiguration Default(string provider)
    {
        return providers.FirstOrDefault(p => p.Name == provider)?.Default()
            ?? throw new ArgumentException($"Unknown provider '{provider}'", nameof(provider));
    }

    public DomainBindingConfiguration Read(string provider, IReadOnlyDictionary<string, string> values)
    {
        var configuration = providers.FirstOrDefault(p => p.Name == provider)?.Default()
               ?? throw new ArgumentException($"Unknown provider '{provider}'", nameof(provider));
        
        configuration.Apply(values);

        return configuration;
    }
}
