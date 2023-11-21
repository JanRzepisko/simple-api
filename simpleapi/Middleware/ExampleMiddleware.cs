using System.Net;
using simpleapi.core.App.Middleware;

namespace simpleapi.Middleware;

public class ExampleMiddleware : IMiddleware
{
    public async Task<HttpListenerContext> Invoke(HttpListenerContext ctx)
    {
        Console.WriteLine("siema wykonało się");
        return ctx;
    }
}