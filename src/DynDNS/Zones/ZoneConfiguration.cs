namespace DynDNS.Zones;

public sealed record ZoneConfiguration(
    string Zone,
    string Provider, 
    bool BindIPv4,
    bool BindIPv6,
    string ApiKey,
    IReadOnlyList<string> Subdomains);
