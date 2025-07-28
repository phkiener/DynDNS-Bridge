namespace DynDNS.Providers.Hetzner;

public sealed class HetznerClient : IProviderClient
{
    public string Name => "Hetzner";

    public Task ApplyAsync(DomainBindingConfiguration configuration)
    {
        return Task.CompletedTask;
    }
}
