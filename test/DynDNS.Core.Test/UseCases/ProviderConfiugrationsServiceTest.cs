using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Test.Framework;

namespace DynDNS.Core.Test.UseCases;

[UseDependencyInjection]
public sealed class ProviderConfiugrationsServiceTest(IProviderConfigurations providerConfigurations, IDomainBindings domainBindings)
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
        await Assert.That(domainBinding!.ConfiguredProvider!.Parameters["Name"]).IsEqualTo("admin");
        await Assert.That(domainBinding!.ConfiguredProvider!.Parameters["Password"]).IsEqualTo("hunter2");
    }
}
