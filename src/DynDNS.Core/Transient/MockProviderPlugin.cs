using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.Transient;

internal sealed class MockProviderPlugin : IProviderPlugin, IProviderClient
{
    public sealed record DnsRecord(Hostname Hostname, DomainFragment Subdomain, DnsRecordType Type, string IpAddress);

    private readonly List<DnsRecord> records = [];

    public string Name => "Mock";

    public IReadOnlyList<string> Parameters { get; } = ["Name", "Password"];

    public IReadOnlyCollection<DnsRecord> Records => records.AsReadOnly();

    public IProviderClient GetClient(IReadOnlyDictionary<string, string> parameters)
    {
        return this;
    }

    public Task CreateRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type, string ipAddress)
    {
        records.RemoveAll(r => r.Hostname == hostname && r.Subdomain == subdomain && r.Type == type);

        var updatedRecord = new DnsRecord(hostname, subdomain, type, ipAddress);
        records.Add(updatedRecord);

        return Task.CompletedTask;
    }

    public Task DeleteRecordAsync(Hostname hostname, DomainFragment subdomain, DnsRecordType type)
    {
        records.RemoveAll(r => r.Hostname == hostname && r.Subdomain == subdomain && r.Type == type);

        return Task.CompletedTask;
    }
}
