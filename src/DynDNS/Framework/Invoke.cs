namespace DynDNS.Framework;

public static class Invoke
{
    public static Func<HttpContext, Task> On<T>(Func<T, CancellationToken, Task> action) where T : class
    {
        return ctx => action(ctx.RequestServices.GetRequiredService<T>(), ctx.RequestAborted);
    }
}
