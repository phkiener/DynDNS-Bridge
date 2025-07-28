namespace DynDNS.Providers.Hetzner;

public sealed class HetznerProvider : IProvider
{
    public string Name => "Hetzner";

    public DomainBindingConfiguration Default()
    {
        return new HetznerBindingConfiguration(zoneId: "", apiKey: "");
    }
}
