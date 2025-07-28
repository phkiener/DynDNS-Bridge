namespace DynDNS.Providers;

public interface IProvider
{
    string Name { get; }
    
    DomainBindingConfiguration Default();
}
