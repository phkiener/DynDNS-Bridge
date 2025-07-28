namespace DynDNS.Providers;

public interface IProviderClient
{
    string Name { get; }
    Task ApplyAsync(DomainBindingConfiguration configuration, IReadOnlyCollection<string> subdomains, string? targetAddressIPv4, string? targetAddressIPv6);
}
