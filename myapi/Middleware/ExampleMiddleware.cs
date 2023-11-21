using System.Net;
using myapi.core.App.Middleware;

namespace myapi.Middleware;

public class ExampleMiddleware : IMiddleware
{
    public async Task<HttpListenerContext> Invoke(HttpListenerContext ctx)
    {
        Console.WriteLine("siema wykonało się");
        return ctx;
    }
}