namespace DynDNS.Providers;

public interface IProviderClient
{
    Task Apply(DomainBindingConfiguration configuration);
}
