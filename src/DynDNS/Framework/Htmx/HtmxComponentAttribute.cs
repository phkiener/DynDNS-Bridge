using System.Diagnostics.CodeAnalysis;

namespace DynDNS.Framework.Htmx;

public abstract class HtmxComponentAttribute(string routeTemplate, HttpMethod method) : Attribute
{
    public string RouteTemplate { get; } = routeTemplate;
    public HttpMethod HttpMethod { get; } = method;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class HtmxGetAttribute : HtmxComponentAttribute
{
    // Don't use a primary ctor - Rider doesn't apply the attribute in that case, only with an explicit CTOR
    public HtmxGetAttribute([StringSyntax("Route")] string routeTemplate) : base(routeTemplate, HttpMethod.Get)
    {
    }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class HtmxPostAttribute : HtmxComponentAttribute
{
    // Don't use a primary ctor - Rider doesn't apply the attribute in that case, only with an explicit CTOR
    public HtmxPostAttribute([StringSyntax("Route")] string routeTemplate) : base(routeTemplate, HttpMethod.Post)
    {
    }
}
