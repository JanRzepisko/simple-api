using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using simpleapi.core.Attributes;
using simpleapi.core.DependencyInjection;
using simpleapi.core.Enums;
using simpleapi.core.Exceptions;
using simpleapi.core.Extensions;
using simpleapi.core.Middleware;
using simpleapi.core.Models;

namespace simpleapi.core.App;

public partial class App : IApp
{
    private HttpListener _server;
    private List<MapModel> Endpoints = new();
    private Assembly EntryPoint;
    internal MethodInfo MethodToResolve;
    private int Port;
    private bool Initialized = false;
    private bool Started = false;
    
    internal readonly List<SingletonService> SingletonServices = new();
    internal readonly List<MultiService> MultiServices = new();
    internal readonly List<object> Middlewares = new();

    public Task Run()
    {
        if (!Initialized)
            throw new Exception("App must be initialized");
        
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
                .FirstOrDefault(x  => x.Name == "Handle" 
                          && x.DeclaringType.GetInterfaces().FirstOrDefault()!.IsGenericType 
                          && x.ReturnType == typeof(Task<>).MakeGenericType(x.DeclaringType.GetInterfaces().FirstOrDefault().GetGenericArguments()[1])
                          && x.DeclaringType.DeclaringType.CustomAttributes.Any(z => z.AttributeType == typeof(ApiAttribute)));
            if (handle is null)
                continue;
            if (handlerType is null)
                throw new HandlerCannotBeRegister(handle.Name);

            var baseOfEndpoint = e.CustomAttributes.FirstOrDefault();
            
            if(baseOfEndpoint is null)
                throw new HandlerCannotBeRegister(handle.Name);
            
            var path = baseOfEndpoint.ConstructorArguments[0].Value!.ToString();
            var method =(Method)e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[1].Value!;

            var command = Activator.CreateInstance(handle.GetParameters()[0].ParameterType);
            var map = new MapModel()
            {
                Handler = handlerType,
                Command = command,
                Method = method,
                Path = path,
                OutputType = handle.ReturnType.GetGenericArguments()[0]
            };
            app.Endpoints.Add(map);
        }

        app.Initialized = true;
        return app;
    }
    private async Task OnRequest()
    {
        var ctx = _server.GetContext();
        this.RunMiddlewares(ctx);
        var body = new StreamReader(ctx.Request.InputStream).ReadToEndAsync();
        var endpoint = Endpoints.FirstOrDefault(c => c.Path == ctx.Request.RawUrl!.Split("?")[0] && c.Method.ToString() == ctx.Request.HttpMethod);
        var handler = BuildHandler(endpoint.Handler);
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
        var responseValue = endpoint.Handler.GetMethods()[0].Invoke(handler,new[]{ command });
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

    private object BuildHandler(Type handlerType)
    {
        var constructorParameters = handlerType.GetConstructors().MaxBy(c => c.GetParameters().Length)?.GetParameters();
        var typesOfParameters = constructorParameters?.Select(c => c.ParameterType).ToList();
        var serviceToInject = new List<object>();
        
        
        foreach (var item in typesOfParameters)
        {
            if (SingletonServices.Any(c => c.Instance.GetType() == item))
            {
                var service = SingletonServices.FirstOrDefault(c =>  c.Instance.GetType() == item).Instance;
                if(service is not null)
                    serviceToInject.Add(service);
            }
            else if (MultiServices.Any(c => c.Interface== item))
            {
                var service = MultiServices.FirstOrDefault(c => c.Interface == item);
                if (service is null) continue;
                
                var createInstanceMethod = service.GetType().GetMethod("CrateInstance").MakeGenericMethod(service.Interface);
                
                var instance = createInstanceMethod.Invoke(service, null);
                serviceToInject.Add(instance);
            }
        }
        var ctorParams = serviceToInject.Select(c => c.GetType()).ToArray();
        var ctor = handlerType.GetConstructor(ctorParams);
        var handler = ctor.Invoke(serviceToInject.ToArray());
        return handler;
    }
    public static object ResolveTask<T>(Task<T?> obj) => obj.Result!;
}