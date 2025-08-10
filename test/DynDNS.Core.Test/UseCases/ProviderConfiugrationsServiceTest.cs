using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Abstractions.Plugins;
using DynDNS.Core.Test.Framework;
using DynDNS.Core.Transient;

namespace DynDNS.Core.Test.UseCases;

[UseDependencyInjection]
public sealed class ProviderConfiugrationsServiceTest(
    IProviderConfigurations providerConfigurations,
    IDomainBindings domainBindings,
    ISubdomains subdomains,
    IProviderPlugin plugin,
    ICurrentAddressProvider currentAddressProvider)
{
    [Test]
    public async Task ReturnsNamesOfRegisteredPlugins()
    {
        var providerNames = providerConfigurations.Providers;

        await Assert.That(providerNames).IsEquivalentTo(["Mock"]);
    }

    [Test]
    public async Task ReturnsParametersOfRegisteredPlugin()
    {
        var parameters = providerConfigurations.GetParameters("Mock");

        await Assert.That(parameters).IsEquivalentTo(["Name", "Password"]);
    }

    [Test]
    public void ThrowsException_WhenReadingParametersOfNonexistentPlugin()
    {
        Assert.Throws<InvalidOperationException>(() => providerConfigurations.GetParameters("Fake"));
    }

    [Test]
    public async Task ThrowsException_WhenDomainBindingToConfigureDoesNotExist()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => providerConfigurations.ConfigureProviderAsync(new DomainBindingId("example.com"), "Mock", new Dictionary<string, string>()));
    }

    [Test]
    public async Task ThrowsException_WhenConfiguringANonexistentPluginForDomainBinding()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => providerConfigurations.ConfigureProviderAsync(id, "Fake", new Dictionary<string, string>()));
    }

    [Test]
    public async Task ThrowsException_WhenNotAllParametersHaveBeenSet()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        var parameters = new Dictionary<string, string>
        {
            ["Name"] = "admin"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => providerConfigurations.ConfigureProviderAsync(id, "Mock", parameters));
    }

    [Test]
    public async Task ThrowsException_WhenUnknownParametersHaveBeenSet()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        var parameters = new Dictionary<string, string>
        {
            ["Name"] = "admin",
            ["Password"] = "hunter2",
            ["Extra"] = "Yeah"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => providerConfigurations.ConfigureProviderAsync(id, "Mock", parameters));
    }

    [Test]
    public async Task ConfiguresProviderForDomainBinding()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        var parameters = new Dictionary<string, string>
        {
            ["Name"] = "admin",
            ["Password"] = "hunter2"
        };

        await providerConfigurations.ConfigureProviderAsync(id, "Mock", parameters);

        var domainBinding = await domainBindings.FindDomainBindingAsync(Hostname.From("example.com"));
        await Assert.That(domainBinding?.ConfiguredProvider).IsNotNull();
        await Assert.That(domainBinding!.ConfiguredProvider!.Name).IsEqualTo("Mock");
        await Assert.That(domainBinding.ConfiguredProvider.Parameters["Name"]).IsEqualTo("admin");
        await Assert.That(domainBinding.ConfiguredProvider.Parameters["Password"]).IsEqualTo("hunter2");
    }

    [Test]
    public async Task DoesNothing_WhenApplyingBindingWithoutSubdomains()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        var parameters = new Dictionary<string, string>
        {
            ["Name"] = "admin",
            ["Password"] = "hunter2"
        };

        await providerConfigurations.ConfigureProviderAsync(id, "Mock", parameters);
        await providerConfigurations.UpdateBindingsAsync(id);

        var client = plugin.GetClient(parameters) as MockProviderPlugin;
        await Assert.That(client!.Records).IsEmpty();
    }

    [Test]
    public async Task CreatesDnsRecordsForSubdomain()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        var parameters = new Dictionary<string, string>
        {
            ["Name"] = "admin",
            ["Password"] = "hunter2"
        };

        await providerConfigurations.ConfigureProviderAsync(id, "Mock", parameters);
        await subdomains.AddSubdomainAsync(id, DomainFragment.From("blog"));
        await subdomains.UpdateSubdomainAsync(id, DomainFragment.From("blog"), SubdomainFlags.A | SubdomainFlags.AAAA);

        await providerConfigurations.UpdateBindingsAsync(id);

        var client = plugin.GetClient(parameters) as MockProviderPlugin;
        await Assert.That(client!.Records).IsEquivalentTo([
            new MockProviderPlugin.DnsRecord(Hostname.From("example.com"), DomainFragment.From("blog"), DnsRecordType.A, (await currentAddressProvider.GetIPv4AddressAsync())!),
            new MockProviderPlugin.DnsRecord(Hostname.From("example.com"), DomainFragment.From("blog"), DnsRecordType.AAAA, (await currentAddressProvider.GetIPv6AddressAsync())!)
        ]);
    }

    [Test]
    public async Task UpdatesDnsRecordsForSubdomain()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        var parameters = new Dictionary<string, string>
        {
            ["Name"] = "admin",
            ["Password"] = "hunter2"
        };

        await providerConfigurations.ConfigureProviderAsync(id, "Mock", parameters);
        await subdomains.AddSubdomainAsync(id, DomainFragment.From("blog"));
        await subdomains.UpdateSubdomainAsync(id, DomainFragment.From("blog"), SubdomainFlags.A | SubdomainFlags.AAAA);
        await providerConfigurations.UpdateBindingsAsync(id);

        (currentAddressProvider as MockAddressProvider)!.IPv4Address = "127.0.0.2";
        (currentAddressProvider as MockAddressProvider)!.IPv6Address = "::2";
        await providerConfigurations.UpdateBindingsAsync(id);

        var client = plugin.GetClient(parameters) as MockProviderPlugin;
        await Assert.That(client!.Records).IsEquivalentTo([
            new MockProviderPlugin.DnsRecord(Hostname.From("example.com"), DomainFragment.From("blog"), DnsRecordType.A, "127.0.0.2"),
            new MockProviderPlugin.DnsRecord(Hostname.From("example.com"), DomainFragment.From("blog"), DnsRecordType.AAAA, "::2")
        ]);
    }

    [Test]
    public async Task DeletesDnsRecordsForSubdomain()
    {
        var id = await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"));
        var parameters = new Dictionary<string, string>
        {
            ["Name"] = "admin",
            ["Password"] = "hunter2"
        };

        await providerConfigurations.ConfigureProviderAsync(id, "Mock", parameters);
        await subdomains.AddSubdomainAsync(id, DomainFragment.From("blog"));
        await subdomains.UpdateSubdomainAsync(id, DomainFragment.From("blog"), SubdomainFlags.A | SubdomainFlags.AAAA);
        await providerConfigurations.UpdateBindingsAsync(id);

        await subdomains.UpdateSubdomainAsync(id, DomainFragment.From("blog"), SubdomainFlags.None);
        await providerConfigurations.UpdateBindingsAsync(id);

        var client = plugin.GetClient(parameters) as MockProviderPlugin;
        await Assert.That(client!.Records).IsEmpty();
    }
}
