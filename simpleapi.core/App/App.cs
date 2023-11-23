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

public class App : IApp
{
    private HttpListener? _server;
    private readonly List<MapModel> _endpoints = new();
    private Assembly? _entryPoint;
    internal MethodInfo MethodToResolve = null!;
    private int _port;
    private bool _initialized = false;
    private bool _started = false;
    
    internal readonly List<SingletonService> SingletonServices = new();
    internal readonly List<MultiService> MultiServices = new();
    internal readonly List<object> Middlewares = new();
    public Task Run()
    {
        if (!_initialized)
            throw new Exception("App must be initialized");
        
        _server = new HttpListener();
        _server.Prefixes.Add($"http://localhost:{_port}/");

        _server.Start();
        Console.WriteLine($"Listening on port {_port}...");

        _started = true;
        while (_started)
        {
            OnRequest();
        }

        return Task.CompletedTask;
    }
    public static App Init<TEntryPoint>(int port)
    {
        var app = new App
        {
            _port = port,
            MethodToResolve = typeof(App).GetMethods().FirstOrDefault(c => c.Name == "ResolveTask")!,
            _entryPoint = typeof(TEntryPoint).Assembly
        };
        var types = typeof(TEntryPoint).Assembly.GetTypes();
        var endpoints = types.Where(c => c.CustomAttributes
            .Any(attributeData =>attributeData.AttributeType == typeof(ApiAttribute))).ToList();
        foreach (var e in endpoints)
        {
            var assembly = typeof(TEntryPoint).Assembly;
            var handlerType = ClassHierarchyExplorer.GetClassesImplementingInterface(typeof(IEndpointModel), assembly)
                .FirstOrDefault(c => 
                    c.GetMethods()
                        .Any(x => x.DeclaringType != null
                                  && x.Name == "Handle" 
                                  && x.DeclaringType!.GetInterfaces().FirstOrDefault()!.IsGenericType 
                                  && x.ReturnType == typeof(Task<>).MakeGenericType(x.DeclaringType.GetInterfaces().FirstOrDefault()!.GetGenericArguments()[1]) 
                                  && x.DeclaringType.DeclaringType!.CustomAttributes.Any(z => z.AttributeType == typeof(ApiAttribute)) 
                                  && x.DeclaringType.DeclaringType.CustomAttributes.FirstOrDefault(z => z.AttributeType == typeof(ApiAttribute))!.ConstructorArguments[0].Value!.ToString()
                                  == e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[0].Value!.ToString() 
                                  && x.DeclaringType.DeclaringType.CustomAttributes.FirstOrDefault(z => z.AttributeType == typeof(ApiAttribute))!.ConstructorArguments[1].Value!.ToString()
                                  == e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[1].Value!.ToString()));

            if(handlerType is null)
                continue;
            
            var handle = handlerType.GetMethods()
                .FirstOrDefault(x  => x.Name == "Handle" 
                          && x.DeclaringType!.GetInterfaces().FirstOrDefault()!.IsGenericType 
                          && x.ReturnType == typeof(Task<>).MakeGenericType(x.DeclaringType.GetInterfaces().FirstOrDefault()!.GetGenericArguments()[1])
                          && x.DeclaringType.DeclaringType!.CustomAttributes.Any(z => z.AttributeType == typeof(ApiAttribute)));
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
            if(command is null)
                continue;
            var map = new MapModel
            {
                Handler = handlerType,
                Command = command,
                Method = method,
                Path = path,
                OutputType = handle.ReturnType.GetGenericArguments()[0]
            };
            app._endpoints.Add(map);
        }

        app._initialized = true;
        return app;
    }
    private async Task OnRequest()
    {
        var ctx = _server!.GetContext();
        this.RunMiddlewares(ctx);
        var body = new StreamReader(ctx.Request.InputStream).ReadToEndAsync();
        var endpoint = _endpoints.FirstOrDefault(c => c.Path == ctx.Request.RawUrl!.Split("?")[0] && c.Method.ToString() == ctx.Request.HttpMethod);
        if (endpoint is null)
        {
            await RequestError.RequestError.Return404(ctx);
            return;
        }
        var handler = BuildHandler(endpoint.Handler);
        object command;
        
        if (endpoint.Method == Method.GET)
        {
            var paramsFromUri = ctx.Request.QueryString;
            command = endpoint.Command;

            var fixedParams = paramsFromUri.AllKeys.Select(c => c!.ToLower()).ToList();
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
                command = JsonConvert.DeserializeObject(await body, endpoint.Command.GetType())!;
            }
            catch (JsonReaderException e)
            {
                await RequestError.RequestError.Return400(ctx, e.Message);
                return;
            }
        }
        object resolvedTask;
        var responseValue = endpoint.Handler.GetMethods()[0].Invoke(handler,new[]{ command });
        var methodToResolve = MethodToResolve.MakeGenericMethod(endpoint.OutputType);
        try
        {
            resolvedTask = methodToResolve.Invoke(this, new[]
            {
                responseValue
            }) ?? throw new Exception("null on invoking");
        }
        catch (Exception)
        {
            await RequestError.RequestError.Return500(ctx);
            return;
        }
        
        var response = JsonConvert.SerializeObject(resolvedTask);
        using var resp = ctx.Response;
        
        var byteResponse = Encoding.UTF8.GetBytes(response);
        await RequestError.RequestError.Return200(ctx, byteResponse);
    }
    private object BuildHandler(Type handlerType)
    {
        var constructorParameters = handlerType.GetConstructors().MaxBy(c => c.GetParameters().Length)?.GetParameters();
        var typesOfParameters = constructorParameters?.Select(c => c.ParameterType).ToList();
        var serviceToInject = new List<object>();
        
        foreach (var item in typesOfParameters!)
        {
            if (SingletonServices.Any(c => c.Instance.GetType() == item || c.Interface == item))
            {
                var service = SingletonServices.FirstOrDefault(c => c.Instance.GetType() == item)?.Instance ??
                              SingletonServices.FirstOrDefault(c =>  c.Interface == item)?.Instance;
                if(service is not null)
                    serviceToInject.Add(service);
            }
            else if (MultiServices.Any(c => c.Interface == item || (c.Interface is null && c.Implementation == item)))
            {
                var service = MultiServices.FirstOrDefault(c => c.Interface == item);
                if (service is null) continue;

                var createInstanceMethod = service.GetType().GetMethod("CrateInstance")!;
                if (service.Interface is null)
                {
                    createInstanceMethod = createInstanceMethod.MakeGenericMethod(service.Interface!);
                }
                else
                {
                    createInstanceMethod = createInstanceMethod.MakeGenericMethod(service.Implementation!);
                }
                
                var instance = createInstanceMethod.Invoke(service, null);
                if (instance is not null) 
                    serviceToInject.Add(instance);
            }
        }
        var ctorParams = serviceToInject.Select(c => c.GetType()).ToArray();
        var ctor = handlerType.GetConstructor(ctorParams);
        object handler;
        if (ctor is not null)
        {
            handler = ctor.Invoke(serviceToInject.ToArray());
        }
        else
        {
            handler = Activator.CreateInstance(handlerType) ?? throw new InvalidOperationException();
        }

        return handler;
    }
    public static object ResolveTask<T>(Task<T?> obj) => obj.Result!;
}