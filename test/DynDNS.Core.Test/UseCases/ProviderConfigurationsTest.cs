using DynDNS.Core.Abstractions;
using DynDNS.Core.Abstractions.Models;
using DynDNS.Core.Test.Framework;

namespace DynDNS.Core.Test.UseCases;

[UseDependencyInjection]
public sealed class ProviderConfigurationsTest(IProviderConfigurations providerConfigurations)
{
    [Test]
    public async Task MockProviderIsFound()
    {
        var providers = providerConfigurations.AvailableProviders;
        var mockProvider = providers.SingleOrDefault(static p => p.Name == MockProvider.Name);

        await Assert.That(mockProvider).IsNotNull();

        var expectedParameter = new ProviderConfigurationParameter(MockProvider.ParameterName, true, ParameterType.Text);
        await Assert.That(mockProvider!.ConfigurationParameters).IsEquivalentTo([expectedParameter]);
    }
}
