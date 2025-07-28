namespace DynDNS.Providers;

public interface IProviderClient
{
    string Name { get; }
    Task ApplyAsync(DomainBindingConfiguration configuration);
}
