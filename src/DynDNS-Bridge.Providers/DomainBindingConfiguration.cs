namespace DynDNS.Providers;

public abstract class DomainBindingConfiguration
{
    public abstract string Provider { get; }

    public abstract IReadOnlyDictionary<string, string> Snapshot();
    public abstract void Apply(IReadOnlyDictionary<string, string> values);
}
