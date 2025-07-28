namespace DynDNS.Providers.Hetzner;

public sealed class HetznerBindingConfiguration(string zoneId, string apiKey) : DomainBindingConfiguration
{
    public override string Provider => "Hetzner";

    public string ZoneId { get; private set; } = zoneId;
    public string ApiKey { get; private set; } = apiKey;

    public override IReadOnlyDictionary<string, string> Snapshot()
    {
        return new Dictionary<string, string>
        {
            [nameof(ZoneId)] = ZoneId,
            ["API Key"] = ApiKey
        };
    }

    public override void Apply(IReadOnlyDictionary<string, string> values)
    {
        ZoneId = values.GetValueOrDefault(nameof(ZoneId), ZoneId);
        ApiKey = values.GetValueOrDefault("API Key", ApiKey);
    }
}
