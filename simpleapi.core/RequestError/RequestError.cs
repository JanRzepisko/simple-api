using System.Net;
using System.Text;

namespace simpleapi.core.RequestError;

public static class RequestError
{
    public static async Task Return200(HttpListenerContext c, byte[] response)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.OK;
        resp.StatusDescription = "Success";
        await resp.OutputStream.WriteAsync(response);
    }    
    public static async Task Return400(HttpListenerContext c, string message = "")
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.BadRequest;
        resp.StatusDescription = "Bad Request";
        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Bad Request " + message));
    }    
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
        resp.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
        resp.StatusDescription = "Method Not Allowed";
        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Method Not Allowed"));
    }

    public static async Task Return500(HttpListenerContext c, string message = "")
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.InternalServerError;
        resp.StatusDescription = "Internal Server Error";
        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Internal Server Error " + message));    
    }
}