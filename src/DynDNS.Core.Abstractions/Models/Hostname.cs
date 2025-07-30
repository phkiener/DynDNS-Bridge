using System.Text.RegularExpressions;

namespace DynDNS.Core.Abstractions.Models;

/// <summary>
/// A base hostname, without subdomains.
/// </summary>
/// <example>
/// Given the URL <c>blog.me.example.com</c>, <c>example.com</c> is the <see cref="Hostname"/> while <c>blog.me</c> is
/// a <see cref="DomainFragment"/>.
/// </example>
public sealed partial record Hostname
{
    private readonly string value;
    
    private Hostname(string value)
    {
        this.value = value;
    }

    /// <summary>
    /// Build a full hostname by prepending <see cref="subdomain"/> to this hostname.
    /// </summary>
    /// <param name="subdomain">The subdomain to prepent.</param>
    /// <returns>A fully built URL</returns>
    /// <example>
    /// <code>
    ///     var host = Hostname.From("example.com");
    ///     var subdomain = DomainFragment.From("blog.me");
    /// 
    ///     host.BuildUrl(subdomain);
    /// </code>
    ///
    /// produces the URL <c>"blog.me.example.com"</c>
    /// </example>
    public string BuildUrl(DomainFragment subdomain)
    {
        return $"{subdomain}.{value}";
    }
    
    /// <inheritdoc />
    public override string ToString() => value;

    public static implicit operator string(Hostname id) => id.value;
    
    /// <summary>
    /// Create a new <see cref="Hostname"/>.
    /// </summary>
    public static Hostname From(string value)
    {
        if (!IsValid(value))
        {
            throw new ArgumentException($"'{value}' is not a valid hostname.", nameof(value));
        }
        
        return new Hostname(value);
    }

    private static bool IsValid(string value) => HostnameRegex().IsMatch(value);
    
    /// <summary>
    /// A regular expression to match a hostname, though not perfectly. Length of the label is
    /// not validated, neither is the validity of the TLD.<br/>
    /// The grammar is as follows:<br/>
    ///     <code>
    ///     hostname    ::= label '.' TLD ;
    ///     label       ::= no-hyphen | (no-hyphen with-hyphen* no-hyphen) ;
    ///     tld         ::= label ;
    ///     no-hyphen   ::= 'a' | 'b' | ... | 'z' | '0' | '1' | ... | '9' ;
    ///     with-hyphen ::= no-hyphen | '-' ;
    ///     </code>
    /// </summary>
    [GeneratedRegex(@"^([a-z0-9]|[a-z0-9][-a-z0-9]*[a-z0-9])\.([a-z0-9]|[a-z0-9][-a-z0-9]*[a-z0-9])$", RegexOptions.IgnoreCase)]
    private static partial Regex HostnameRegex();
}
