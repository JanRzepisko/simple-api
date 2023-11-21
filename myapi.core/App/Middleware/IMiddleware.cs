using System.Net;

namespace myapi.core.App.Middleware;

public interface IMiddleware
{
    Task<HttpListenerContext> Invoke(HttpListenerContext ctx);
}