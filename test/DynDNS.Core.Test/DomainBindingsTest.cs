using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Test.Framework;

namespace DynDNS.Core.Test;

[UseDependencyInjection]
public sealed class DomainBindingsTest(IDomainBindings domainBindings)
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
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"), "Mock");
        
        var bindings = await domainBindings.GetDomainBindingsAsync();
        var binding = bindings.SingleOrDefault();

        await Assert.That(binding).IsNotNull();
        await Assert.That(binding!.Id).IsEqualTo(new DomainBindingId("example.com"));
        await Assert.That(binding.Domain).IsEqualTo(Hostname.From("example.com"));
        await Assert.That(binding.Provider).IsEqualTo("Mock");
        await Assert.That(binding.Subdomains).IsEmpty();
        await Assert.That(binding.ProviderConfigurationParameters).IsEmpty();
    }

    [Test]
    public async Task AddingDomainBinding_IsIdempotent()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"), "Mock");
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"), "Mock");
        
        var bindings = await domainBindings.GetDomainBindingsAsync();
        await Assert.That(bindings).HasCount(1);
    }

    [Test]
    public async Task DomainBindingsCanBeRemoved()
    {
        await domainBindings.CreateDomainBindingAsync(Hostname.From("example.com"), "Mock");
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
}
