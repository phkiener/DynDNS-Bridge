using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DynDNS.Framework;

public sealed class RazorComponentActionResult<T> : RazorComponentResult<T>, IActionResult where T : IComponent
{
    public RazorComponentActionResult()
    {
        
    }
    
    public RazorComponentActionResult(IReadOnlyDictionary<string, object?> parameters) : base(parameters)
    {
        
    }
    
    public Task ExecuteResultAsync(ActionContext context)
    {
        return ExecuteAsync(context.HttpContext);
    }
}
