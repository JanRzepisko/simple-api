using System.Net;
using myapi.core.App.Middleware;

namespace myapi.core.App;

public partial class App : IAppMiddleware
{
    public readonly List<object?> Middlewares = new List<object?>();
    public App AddMiddleware<TMiddleware>() where TMiddleware : IMiddleware
    {
        Middlewares.Add(Activator.CreateInstance<TMiddleware>());
        return this;
    }

    private void RunMiddlewares(HttpListenerContext? ctx)
    {
        foreach (var middleware in Middlewares)
        {
            var res = middleware.GetType().GetMethod("Invoke").Invoke(middleware, new object[] { ctx });
            var x = MethodToResolve.MakeGenericMethod(typeof(HttpListenerContext)).Invoke(middleware, new[] { res });
            ctx = x as HttpListenerContext;
        }
    }
}