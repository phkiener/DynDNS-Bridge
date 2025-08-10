using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;
using DynDNS.Core.Test.Framework;
using DynDNS.Core.Transient;

namespace DynDNS.Core.Test.UseCases;

[UseDependencyInjection]
public sealed class SubdomainsTest(
    ISubdomains subdomains,
    IDomainBindings domainBindings,
    IProviderConfigurations providerConfigurations,
    IProviderPlugin plugin,
    ICurrentAddressProvider currentAddressProvider)
{
    [Test]
    public async Task ThrowsException_WhenAddingSubdomain_ToNonexistentDomainBinding()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog")));
    }

    [Test]
    public async Task ThrowsException_WhenRemovingSubdomain_FromNonexistentDomainBinding()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => subdomains.RemoveSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog")));
    }

    [Test]
    public async Task ThrowsException_WhenModyfingSubdomain_OnNonexistentDomainBinding()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => subdomains.UpdateSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"), SubdomainFlags.A));
    }

    [Test]
    public async Task ThrowsException_WhenModyfingSubdomain_ThatDoesNotExistOnDomainBinding()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => subdomains.UpdateSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"), SubdomainFlags.A));
    }

    [Test]
    public async Task AddsSubdomainToDomainBinding()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));

        var domainBinding = await domainBindings.FindDomainBindingAsync(Hostname.From("example.com"));

        var expectedSubdomain = new DomainBinding.Subdomain(DomainFragment.From("blog"), SubdomainFlags.None);
        await Assert.That(domainBinding!.Subdomains).IsEquivalentTo([expectedSubdomain]);
    }

    [Test]
    public async Task AddingSubdomains_IsIdempotent()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));
        await subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));

        var domainBinding = await domainBindings.FindDomainBindingAsync(Hostname.From("example.com"));

        await Assert.That(domainBinding!.Subdomains).HasCount(1);
    }

    [Test]
    public async Task RemovesSubdomainFromDomainBinding()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));

        await subdomains.RemoveSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));

        var domainBinding = await domainBindings.FindDomainBindingAsync(Hostname.From("example.com"));
        await Assert.That(domainBinding!.Subdomains).IsEmpty();
    }

    [Test]
    public async Task NothingHappens_WhenRemovingNonexistentSubdomain()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));

        await subdomains.RemoveSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));

        var domainBinding = await domainBindings.FindDomainBindingAsync(Hostname.From("example.com"));
        await Assert.That(domainBinding!.Subdomains).IsEmpty();
    }

    [Test]
    public async Task SubdomainCanBeUpdated()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));

        await subdomains.UpdateSubdomainAsync(
            new DomainBindingId("example.com"),
            DomainFragment.From("blog"),
            SubdomainFlags.A | SubdomainFlags.AAAA);

        var domainBinding = await domainBindings.FindDomainBindingAsync(Hostname.From("example.com"));
        await Assert.That(domainBinding!.Subdomains.Single().Flags).IsEqualTo(SubdomainFlags.A | SubdomainFlags.AAAA);
    }

    [Test]
    public async Task DnsEntriesAreRemoved_WhenSubdomainIsRemoved()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"));
        await subdomains.UpdateSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("blog"), SubdomainFlags.A);
        await subdomains.AddSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("public"));
        await subdomains.UpdateSubdomainAsync(new DomainBindingId("example.com"), DomainFragment.From("public"), SubdomainFlags.A);
        await providerConfigurations.ConfigureProviderAsync(
            new DomainBindingId("example.com"),
            "Mock",
            new Dictionary<string, string> { ["Name"] = "admin", ["Password"] = "hunter2" });

        await providerConfigurations.UpdateBindingsAsync(new DomainBindingId("example.com"));
        await subdomains.RemoveSubdomainAsync(new DomainBindingId("example.com"),  DomainFragment.From("blog"));

        var client = plugin.GetClient(new Dictionary<string, string> { ["Name"] = "admin", ["Password"] = "hunter2" }) as MockProviderPlugin;
        await Assert.That(client!.Records).IsEquivalentTo([
            new MockProviderPlugin.DnsRecord(Hostname.From("example.com"), DomainFragment.From("blog"), DnsRecordType.A, (await currentAddressProvider.GetIPv4AddressAsync())!),
        ]);
    }
}
