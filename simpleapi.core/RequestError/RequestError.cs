using System.Net;
using System.Text;
using simpleapi.core.DefaultFeatures;

namespace simpleapi.core.RequestError;

public static class RequestError
{
    public static async Task Return200(this App.App app, HttpListenerContext c, object response)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.OK;
        resp.StatusDescription = "Success";
        var res = Encoding.UTF8.GetBytes(app.PrepareResponse(response, 200));
        await resp.OutputStream.WriteAsync(res);
    }  
    public static async Task ReturnHTML(this App.App app, HttpListenerContext c, string body)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.OK;
        resp.StatusDescription = "Success";
        var res = Encoding.UTF8.GetBytes(body);
        await resp.OutputStream.WriteAsync(res);
    }  
    public static async Task Return400(this App.App app, HttpListenerContext c, string message = "")
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.BadRequest;
        resp.StatusDescription = "Bad Request";
        var res = Encoding.UTF8.GetBytes(app.PrepareResponse(message, 400));
        await resp.OutputStream.WriteAsync(res);
    }    
    public static async Task Return404(this App.App app, HttpListenerContext c)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.NotFound;
        resp.StatusDescription = "Not Found";
        var res = Encoding.UTF8.GetBytes(app.PrepareResponse("Not Found", 404));
        await resp.OutputStream.WriteAsync(res);
    }    
    public static async Task Return405(this App.App app, HttpListenerContext c)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
        resp.StatusDescription = "Method Not Allowed";
        var res = Encoding.UTF8.GetBytes(app.PrepareResponse("Method Not Allowed", 405));
        await resp.OutputStream.WriteAsync(res);
    }

    public static async Task Return500(this App.App app, HttpListenerContext c, string message = "")
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.InternalServerError;
        resp.StatusDescription = "Internal Server Error";
        var res = Encoding.UTF8.GetBytes(app.PrepareResponse(message, 500));
        await resp.OutputStream.WriteAsync(res);    
    }
}