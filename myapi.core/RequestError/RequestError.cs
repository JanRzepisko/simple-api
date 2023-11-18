using System.Net;
using System.Text;

namespace myapi.core.RequestError;

public static class RequestError
{
    public static async Task Return404(HttpListenerContext c)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.NotFound;
        resp.StatusDescription = "Not Found";
        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Not Found"));
    }    
    public static async Task Return405(HttpListenerContext c)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.NotFound;
        resp.StatusDescription = "Not Found";
        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Bad Request"));
    }
}