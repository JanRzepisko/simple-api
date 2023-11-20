using myapi.core.App.Middleware;
using myapi.core.Middlewares;

namespace myapi.core.App;

public partial class App : IAppMiddleware
{
    public List<object?> Middlewares = new List<object?>();
    public App AddMiddleware<TMiddleware>() where TMiddleware : IMiddleware
    {
        Middlewares.Add(Activator.CreateInstance<TMiddleware>());
        return this;
    }
}