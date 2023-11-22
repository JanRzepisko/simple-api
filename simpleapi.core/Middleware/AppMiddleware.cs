using System.Net;

namespace simpleapi.core.Middleware;

public static class AppMiddleware
{
    public static App.App AddMiddleware<TMiddleware>(this App.App app) where TMiddleware : IMiddleware
    {
        app.Middlewares.Add(Activator.CreateInstance<TMiddleware>());
        return app;
    }

    internal static void RunMiddlewares(this App.App app, HttpListenerContext? ctx)
    {
        foreach (var middleware in app.Middlewares)
        {
            var res = middleware.GetType().GetMethod("Invoke").Invoke(middleware, new object[] { ctx });
            var x = app.MethodToResolve.MakeGenericMethod(typeof(HttpListenerContext)).Invoke(middleware, new[] { res });
            ctx = x as HttpListenerContext;
        }
    }
}