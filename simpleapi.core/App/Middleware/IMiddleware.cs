using System.Net;

namespace simpleapi.core.App.Middleware;

public interface IMiddleware
{
    Task<HttpListenerContext> Invoke(HttpListenerContext ctx);
}