namespace DynDNS.Zones;

public sealed record ZoneConfiguration(string Zone, string Provider, IReadOnlyDictionary<string, string> Parameters, IReadOnlyList<string> Subdomains);
