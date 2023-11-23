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

    internal static void RunPreMiddlewares(this App.App app, HttpListenerContext? ctx)
    {
        foreach (var middleware in app.PreMiddlewares)
        {
            var res = middleware.GetType().GetMethod("Invoke").Invoke(middleware, new object[] { ctx });
            var x = app.MethodToResolve.MakeGenericMethod(typeof(HttpListenerContext)).Invoke(middleware, new[] { res });
            ctx = x as HttpListenerContext;
        }
    }
    internal static void RunPostMiddlewares(this App.App app, HttpListenerContext? ctx)
    {
        foreach (var middleware in app.PostMiddlewares)
        {
            var res = middleware.GetType().GetMethod("Invoke").Invoke(middleware, new object[] { ctx });
            var x = app.MethodToResolve.MakeGenericMethod(typeof(HttpListenerContext)).Invoke(middleware, new[] { res });
            ctx = x as HttpListenerContext;
        }
    }
}