using DynDNS.Core.Abstractions;
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
}
