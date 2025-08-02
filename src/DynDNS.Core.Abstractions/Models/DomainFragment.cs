using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DynDNS.Core.Abstractions.Models;

/// <summary>
/// A <em>fragment</em> of a hostname - but a full hostname is also allowed.
/// </summary>
/// <example>
/// Given the URL <c>blog.me.example.com</c>, both <c>blog</c> and <c>blog.me</c> would be allowed fragments.
/// </example>
public sealed partial record DomainFragment
{
    private readonly string value;

    private DomainFragment(string value)
    {
        this.value = value;
    }

    /// <inheritdoc />
    public override string ToString() => value;

    public static implicit operator string(DomainFragment id) => id.value;

    /// <summary>
    /// Create a new <see cref="DomainFragment"/>.
    /// </summary>
    /// <param name="value">The text to parse.</param>
    public static DomainFragment From(string value)
    {
        return TryCreate(value, out var result) ? result : throw new ArgumentException($"'{value}' is not a valid domain fragment.", nameof(value));
    }

    /// <summary>
    /// Try to create a new <see cref="DomainFragment"/>.
    /// </summary>
    /// <param name="value">The text to parse.</param>
    /// <param name="result">The created <see cref="DomainFragment"/> - or null.</param>
    /// <returns><c>true</c> if the domain fragment was created, <c>false</c> otherwise.</returns>
    public static bool TryCreate(string value, [NotNullWhen(true)] out DomainFragment? result)
    {
        if (!IsValid(value))
        {
            result = null;
            return false;
        }

        result = new DomainFragment(value);
        return true;
    }

    private static bool IsValid(string value) => DomainFragmentRegex().IsMatch(value);

    /// <summary>
    /// A regular expression to match a hostname, though not perfectly. Length of the label is
    /// not validated.<br/>
    /// The grammar is as follows:<br/>
    ///     <code>
    ///     hostname    ::= label ( '.' label )* ;
    ///     label       ::= no-hyphen | (no-hyphen with-hyphen* no-hyphen) ;
    ///     no-hyphen   ::= 'a' | 'b' | ... | 'z' | '0' | '1' | ... | '9' ;
    ///     with-hyphen ::= no-hyphen | '-' ;
    ///     </code>
    /// </summary>
    [GeneratedRegex(
        @"^([a-z0-9]|[a-z0-9][-a-z0-9]*[a-z0-9])(\.([a-z0-9]|[a-z0-9][-a-z0-9]*[a-z0-9]))*$",
        RegexOptions.IgnoreCase)]
    private static partial Regex DomainFragmentRegex();
}
