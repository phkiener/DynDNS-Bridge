namespace DynDNS.Core.Abstractions.Models;

/// <summary>
/// Unqiue identifier for a domain binding.
/// </summary>
/// <param name="Value">The underlying value.</param>
public readonly record struct DomainBindingId(string Value)
{
    public static implicit operator string(DomainBindingId id) => id.Value;
    public static explicit operator DomainBindingId(string id) => new(id);
}
