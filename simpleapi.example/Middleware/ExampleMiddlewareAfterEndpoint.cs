using System.Net;
using simpleapi.core.Middleware;

namespace simpleapi.example.Middleware;

public class ExampleMiddlewareAfterEndpoint : IMiddleware
{
    public async Task<HttpListenerContext> Invoke(HttpListenerContext ctx)
    {
        Console.WriteLine("siema już po");
        return ctx;
    }
}