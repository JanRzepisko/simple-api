using System.Diagnostics;
using System.Net;

namespace simpleapi.core.Middleware;

public static class AppMiddleware
{
    public static App.App RegisterPreMiddleware<TMiddleware>(this App.App app) where TMiddleware : IMiddleware
    {
        app.PreMiddlewares.Add(Activator.CreateInstance<TMiddleware>());
        return app;
    }
    public static App.App RegisterPostMiddleware<TMiddleware>(this App.App app) where TMiddleware : IMiddleware
    {
        app.PostMiddlewares.Add(Activator.CreateInstance<TMiddleware>());
        return app;
    }

    internal static void RunPreMiddlewares(this App.App app, HttpListenerContext ctx)
    {
        foreach (var middleware in app.PreMiddlewares)
        {
            var res = ((middleware as IMiddleware)!).Invoke(ctx);
            ctx = res.Result ?? throw new InvalidOperationException();
        }
    }
    internal static void RunPostMiddlewares(this App.App app, HttpListenerContext ctx)
    {
        foreach (var middleware in app.PostMiddlewares)
        {
            var res = ((middleware as IMiddleware)!).Invoke(ctx);
            ctx = res.Result ?? throw new InvalidOperationException();
        }
    }
}