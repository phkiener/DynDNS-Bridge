namespace DynDNS.Providers;

public sealed class ClientProvider(IEnumerable<IProviderClient> clients)
{
    public IProviderClient Client(string provider)
    {
        return clients.FirstOrDefault(c => c.Name == provider)
               ?? throw new ArgumentException($"Unknown provider '{provider}'", nameof(provider));
    }
}
