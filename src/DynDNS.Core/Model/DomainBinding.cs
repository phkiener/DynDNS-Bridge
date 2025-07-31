using DynDNS.Core.Abstractions.Models;

namespace DynDNS.Core.Model;

/// <summary>
/// A <em>domain binding</em>.
/// </summary>
/// <param name="id">Unique identifier for the domain binding.</param>
/// <param name="hostname">The domain which is to be configured.</param>
/// <param name="subdomains">All subdomains to configure on the domain.</param>
public sealed class DomainBinding(
    DomainBindingId id,
    Hostname hostname,
    IReadOnlyCollection<Subdomain> subdomains)
{
    private readonly Dictionary<DomainFragment, Subdomain> subdomains = subdomains.ToDictionary(static s => s.Name, static s => s);

    /// <summary>
    /// Create a new domain binding.
    /// </summary>
    /// <param name="hostname">The domain which is to be configured.</param>
    public DomainBinding(Hostname hostname)
        : this(
            id: new DomainBindingId(hostname),
            hostname: hostname,
            subdomains: [])
    {

    }

    /// <summary>
    /// Unique identifier for the domain binding.
    /// </summary>
    public DomainBindingId Id { get; } = id;

    /// <summary>
    /// The domain which is to be configured.
    /// </summary>
    public Hostname Hostname { get; } = hostname;

    /// <summary>
    /// All <see cref="Subdomain"/>s to configure on the domain.
    /// </summary>
    public IReadOnlyList<Subdomain> Subdomains => subdomains.Values.ToList().AsReadOnly();

    /// <summary>
    /// Add a new subdomain to this binding.
    /// </summary>
    /// <param name="name">The subdomain to add.</param>
    public void AddSubdomain(DomainFragment name)
    {
        if (subdomains.ContainsKey(name))
        {
            return;
        }

        var subdomain = new Subdomain(name);
        subdomains.Add(name, subdomain);
    }

    /// <summary>
    /// Find a configured subdomain by its name.
    /// </summary>
    /// <param name="name">The subdomain to retrieve.</param>
    /// <returns>The found <see cref="Subdomain"/> or <c>null</c>.</returns>
    public Subdomain? FindSubdomain(DomainFragment name)
    {
        return subdomains.GetValueOrDefault(name);
    }

    /// <summary>
    /// Remove a new subdomain from this binding.
    /// </summary>
    /// <param name="name">The subdomain to remove.</param>
    public void RemoveSubdomain(DomainFragment name)
    {
        subdomains.Remove(name);
    }
}
