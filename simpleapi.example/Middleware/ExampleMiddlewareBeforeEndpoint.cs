using System.Net;
using simpleapi.core.Middleware;

namespace simpleapi.example.Middleware;

public class ExampleMiddlewareBeforeEndpoint : IMiddleware
{
    public async Task<HttpListenerContext> Invoke(HttpListenerContext ctx)
    {
        Console.WriteLine("siema zaraz coś się stanie lub nie xd");
        return ctx;
    }
}