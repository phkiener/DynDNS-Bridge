using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;
using DynDNS.Core.Test.Framework;
using DynDNS.Core.Transient;

namespace DynDNS.Core.Test.UseCases;

[UseDependencyInjection]
public sealed class DomainBindingsTest(
    IDomainBindings domainBindings,
    ISubdomains subdomains,
    IProviderConfigurations providerConfigurations,
    IProviderPlugin plugin)
{
    [Test]
    public async Task HasEmptyListOfBindings()
    {
        var bindings = await domainBindings.GetDomainBindingsAsync();

        await Assert.That(bindings).IsEmpty();
    }

    [Test]
    public async Task CanCreateDomainBinding()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));

        var binding = await domainBindings.FindDomainBindingAsync(Hostname.From("example.com"));

        await Assert.That(binding).IsNotNull();
        await Assert.That(binding!.Id).IsEqualTo(new DomainBindingId("example.com"));
        await Assert.That(binding.Domain).IsEqualTo(Hostname.From("example.com"));
        await Assert.That(binding.Subdomains).IsEmpty();
    }

    [Test]
    public async Task AddingDomainBinding_IsIdempotent()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));

        var bindings = await domainBindings.GetDomainBindingsAsync();
        await Assert.That(bindings).HasCount(1);
    }

    [Test]
    public async Task DomainBindingsCanBeRemoved()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await domainBindings.RemoveDomainBindingAsync(new DomainBindingId("example.com"));

        var bindings = await domainBindings.GetDomainBindingsAsync();
        await Assert.That(bindings).IsEmpty();
    }

    [Test]
    public async Task NothingHappens_WhenRemovingNonexistentBinding()
    {
        await domainBindings.RemoveDomainBindingAsync(new DomainBindingId("example.com"));

        var bindings = await domainBindings.GetDomainBindingsAsync();
        await Assert.That(bindings).IsEmpty();
    }

    [Test]
    public async Task DnsEntriesAreRemoved_WhenDomainBindingIsRemoved()
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
        await domainBindings.RemoveDomainBindingAsync(new DomainBindingId("example.com"));

        var client = plugin.GetClient(new Dictionary<string, string> { ["Name"] = "admin", ["Password"] = "hunter2" }) as MockProviderPlugin;
        await Assert.That(client!.Records).IsEmpty();
    }
}
