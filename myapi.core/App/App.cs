using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using myapi.core.Attributes;
using myapi.core.Enums;
using myapi.core.Exceptions;
using myapi.core.Extensions;
using myapi.core.Models;
using Newtonsoft.Json;


namespace myapi.core.App;

public partial class App : IApp
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
    public static App Init<TEntryPoint>(int port)
    {
        var app = new App();
        
        app.Port = port;   
        app.MethodToResolve = typeof(App).GetMethods().FirstOrDefault(c => c.Name == "ResolveTask")!;
        app.EntryPoint = typeof(TEntryPoint).Assembly;
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
                                  && x.DeclaringType.DeclaringType.CustomAttributes.Any(z => z.AttributeType == typeof(ApiAttribute))
                                  && x.DeclaringType.DeclaringType.CustomAttributes.FirstOrDefault(z => z.AttributeType == typeof(ApiAttribute)).ConstructorArguments[0].Value!.ToString()
                                        == e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[0].Value!.ToString()
                                  && x.DeclaringType.DeclaringType.CustomAttributes.FirstOrDefault(z => z.AttributeType == typeof(ApiAttribute)).ConstructorArguments[1].Value!.ToString()
                                        == e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[1].Value!.ToString()));

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
            var method =(Method)e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[1].Value!;

            if (handle is null)
            {
                continue;
            }
            
            var command = Activator.CreateInstance(handle.GetParameters()[0].ParameterType);
            var map = new MapModel()
            {
                Handler = app.EntryPoint.CreateInstance(handlerType.FullName!),
                Command = command,
                Method = method,
                Path = path,
                OutputType = handle.ReturnType.GetGenericArguments()[0]
            };
            app.Endpoints.Add(map);
        }

        app.Inited = true;
        return app;
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
            try
            {
                command = JsonConvert.DeserializeObject(await body, endpoint.Command.GetType());
            }
            catch (JsonReaderException e)
            {
                RequestError.RequestError.Return400(ctx, e.Message);
                return;
            }
        }
        var resolvedTask = new object();
        var responseValue = endpoint.Handler!.GetType().GetMethods()[0].Invoke(endpoint.Handler,new object?[]{ command });
        var methodToResolve = MethodToResolve.MakeGenericMethod(endpoint.OutputType);
        try
        {
            resolvedTask = methodToResolve.Invoke(this, new[]
            {
                responseValue
            });
        }
        catch (Exception e)
        {
            RequestError.RequestError.Return500(ctx);
            return;
        }

        
        
        var response = JsonConvert.SerializeObject(resolvedTask);
        using var resp = ctx.Response;
        
        var byteResponse = Encoding.UTF8.GetBytes(response);
        RequestError.RequestError.Return200(ctx, byteResponse);
    }
    public static object ResolveTask<T>(Task<T?> obj) => obj.Result!;
}