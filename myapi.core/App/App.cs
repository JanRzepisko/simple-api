using System.Net;
using System.Reflection;
using System.Text;
using myapi.core.Attributes;
using myapi.core.Enums;
using myapi.core.Exceptions;
using myapi.core.Extensions;
using myapi.core.Models;
using Newtonsoft.Json;


namespace myapi.core.App;

public class App : IApp
{
    private HttpListener _server;
    private List<MapModel> Endpoints = new();
    private Assembly EntryPoint;
    private MethodInfo MethodToResolve;
    private int Port;
    private bool Inited = false;
    private bool Started = false;
    
    public Task Run()
    {
        if (!Inited)
            throw new Exception("App must be inited");
        _server = new HttpListener();
        _server.Prefixes.Add($"http://localhost:{Port}/");

        _server.Start();
        Console.WriteLine($"Listening on port {Port}...");

        Started = true;
        while (Started)
        {
            OnRequest();
        }

        return Task.CompletedTask;
    }
    public App Init<TEntryPoint>(int port)
    {
        Port = port;   
        MethodToResolve = typeof(App).GetMethods().FirstOrDefault(c => c.Name == "ResolveTask")!;
        EntryPoint = typeof(TEntryPoint).Assembly;
        var types = typeof(TEntryPoint).Assembly.GetTypes();
        var endpoints = types.Where(c => c.CustomAttributes
            .Any(attributeData =>attributeData.AttributeType == typeof(ApiAttribute))).ToList();
        foreach (var e in endpoints)
        {
            var assembly = typeof(TEntryPoint).Assembly;
            var handlerType = ClassHierarchyExplorer.GetClassesImplementingInterface(typeof(IEndpointModel), assembly)
                .FirstOrDefault(c => 
                    c.GetMethods()
                        .Any(x => x.Name == "Handle" 
                                  && x.DeclaringType.GetInterfaces().FirstOrDefault()!.IsGenericType 
                                  && x.ReturnType == typeof(Task<>).MakeGenericType(x.DeclaringType.GetInterfaces().FirstOrDefault().GetGenericArguments()[1])
                                  && x.DeclaringType.DeclaringType.CustomAttributes.Any(z => z.AttributeType == typeof(ApiAttribute))));

            var handle = handlerType.GetMethods()
                .First(x => x.Name == "Handle" 
                          && x.DeclaringType.GetInterfaces().FirstOrDefault()!.IsGenericType 
                          && x.ReturnType == typeof(Task<>).MakeGenericType(x.DeclaringType.GetInterfaces().FirstOrDefault().GetGenericArguments()[1])
                          && x.DeclaringType.DeclaringType.CustomAttributes.Any(z => z.AttributeType == typeof(ApiAttribute)));
            
            if (handlerType is null)
                throw new HandlerCannotBeRegister(handle.Name);

            var baseOfEndpoint = e.CustomAttributes.FirstOrDefault();
            
            if(baseOfEndpoint is null)
                throw new HandlerCannotBeRegister(handle.Name);
            
            var path = baseOfEndpoint.ConstructorArguments[0].Value!.ToString();
            var method =(Method)e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[1].Value! ;

            if (handle is null)
            {
                continue;
            }
            
            var command = Activator.CreateInstance(handle.GetParameters()[0].ParameterType);
            var map = new MapModel()
            {
                Handler = this.EntryPoint.CreateInstance(handlerType.FullName!),
                Command = command,
                Method = method,
                Path = path,
                OutputType = handle.ReturnType.GetGenericArguments()[0]
            };
            Endpoints.Add(map);
        }

        Inited = true;
        return this;
    }
    private async Task OnRequest()
    {
        var ctx = _server.GetContext();
        var body = new StreamReader(ctx.Request.InputStream).ReadToEndAsync();
        var endpoint = Endpoints.FirstOrDefault(c => c.Path == ctx.Request.RawUrl.Split("?")[0] && c.Method.ToString() == ctx.Request.HttpMethod);
        if (endpoint is null)
        {
            RequestError.RequestError.Return404(ctx);
            return;
        }

        object command = null!;
        
        if (endpoint.Method == Method.GET)
        {
            var paramsFromUri = ctx.Request.QueryString;



            command = endpoint.Command;

            var fixedParams = paramsFromUri.AllKeys.Select(c => c.ToLower()).ToList();
            foreach (var field in command.GetType().GetProperties())
            {
                var value = paramsFromUri[fixedParams.FirstOrDefault(c => c == field.Name.ToLower())];
                command = ParamsParser.ParseToType(field, value, field.PropertyType, command);
            }
        }
        else
        {
            command = JsonConvert.DeserializeObject(await body, endpoint.Command.GetType());
        }

        var responseValue = endpoint.Handler!.GetType().GetMethods()[0].Invoke(endpoint.Handler,new object?[]{ command });
        var methodToResolve = MethodToResolve.MakeGenericMethod(endpoint.OutputType);
        var resolvedTask = methodToResolve.Invoke(this, new object?[]
        {
            responseValue
        });
        
        
        var response = JsonConvert.SerializeObject(resolvedTask);
        using var resp = ctx.Response;
        resp.StatusCode = (int) HttpStatusCode.Accepted;
        resp.StatusDescription = "OK";
        var byteResponse = Encoding.UTF8.GetBytes(response);
        resp.ContentLength64 = byteResponse.Length;
        resp.OutputStream.Write(byteResponse);
        var output = resp.OutputStream;
        await output.WriteAsync(byteResponse,0,byteResponse.Length);
        output.Close();
    }

    
    public static object ResolveTask<T>(Task<T?> obj) => obj.Result!;
}