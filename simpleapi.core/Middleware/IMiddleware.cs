using System.Net;

namespace simpleapi.core.Middleware;

public interface IMiddleware
{
    Task<HttpListenerContext> Invoke(HttpListenerContext ctx);
}