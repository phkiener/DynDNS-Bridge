using DynDNS.Core.Abstractions.Plugins;

namespace DynDNS.Core.Test.Framework;

public sealed class MockProviderPlugin : IProviderPlugin
{
    public string Name => "Mock";

    public IReadOnlyList<string> Parameters { get; } = ["Name", "Password"];
}
