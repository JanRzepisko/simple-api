namespace myapi.core.App.Middleware;

public interface IAppMiddleware
{
    App AddMiddleware<TMiddleware>() where TMiddleware : IMiddleware;
}